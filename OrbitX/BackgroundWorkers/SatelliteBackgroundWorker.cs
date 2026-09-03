using Core.Modules.SGP4Data.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using OrbitX.BackgroundWorkers.Helper;
using OrbitX.SignalRHubs;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;

namespace OrbitX.BackgroundWorkers
{
    public class SatelliteBackgroundWorker : BackgroundService
    {      
        private readonly IServiceProvider _serviceProvider; // Вызов провайдера для Scoped
        private static readonly ConcurrentDictionary<int, (CancellationTokenSource cts, int counter)> _satelliteThreads = new(); // Адресная книга. ID спутника -> Токен отмены его потока данных
        private readonly IHubContext<SignalRHub> _hubContext; // Связываем с SignalR      
        private readonly ILogger<SatelliteBackgroundWorker> _logger; // Логгер
        private CancellationToken _serverStoppingToken; // Ссылка на токен остановки всего сервера

        // Внедряем логгер для отслеживания тактов в консоли
        public SatelliteBackgroundWorker(ILogger<SatelliteBackgroundWorker> logger, IHubContext<SignalRHub> hubContext, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _hubContext = hubContext;
            _serviceProvider = serviceProvider;
        }

        // Метод вызывается из SignalR при первом подключении к спутнику
        public void OnSatelliteWatched(int noradId)
        {
            while (true)
            {
                // Связываем токен спутника с токеном сервера
                // Поток умрет или при вызове OnSatelliteUnwatched, или при выключении всего сервера
                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_serverStoppingToken);

                bool addTaskRun = _satelliteThreads.TryAdd(noradId, (linkedCts, 1));

                if (addTaskRun)
                {
                    // Если успешно добавили — запускаем поток расчета. Токен ушел в словарь
                    _logger.LogInformation($"Инициализация сокета для спутника: {noradId}");

                    _ = Task.Run(() => StartSatelliteStreamingThread(noradId, linkedCts.Token, linkedCts), linkedCts.Token);

                    return;
                }
                else
                {
                    if (!_satelliteThreads.TryGetValue(noradId, out var oldValue))
                    {
                        linkedCts.Dispose();
                        continue; // Спутника удалил какой-то из потокв, идем на второй круг
                    } 
                    
                    int updatedUserCount = oldValue.counter + 1;
                    var newValue = (oldValue.cts, updatedUserCount);

                    if (_satelliteThreads.TryUpdate(noradId, newValue, oldValue))
                    {
                        linkedCts.Dispose();
                        return;
                    }
                    else continue;
                }
            }                     
        }

        // Метод вызывается из SignalR при отключении
        public void OnSatelliteUnwatched(int noradId)
        {
            // Бесконечный цикл нужен только на случай, если другой поток вклинится,
            // чтобы мы просто прокрутили логику заново с актуальными цифрами
            while (true)
            {
                // Достаем текущее состояние спутника из словаря
                if (!_satelliteThreads.TryGetValue(noradId, out var oldValue))
                {
                    return; // Спутника уже нет, выходим
                }

                // Уменьшаем счетчик зрителей на 1
                int updatedUserCount = oldValue.counter - 1;

                if (updatedUserCount > 0)
                {
                    // --- СЦЕНАРИЙ А: Спутник еще кто-то смотрит ---
                    var newState = (oldValue.cts, updatedUserCount);

                    // TryUpdate меняет старое значение на новое
                    // Он сработает, ТОЛЬКО если в словаре всё еще лежит oldValue
                    if (_satelliteThreads.TryUpdate(noradId, newState, oldValue))
                    {
                        _logger.LogInformation($"Пользователь ушел. Осталось: {updatedUserCount}");
                        return; // Успешно обновили, выходим
                    }
                    // Если TryUpdate вернул false — значит, кто-то вклинился параллельно
                    // Цикл while автоматически уйдет на вторую попытку
                }
                else
                {
                    // --- СЦЕНАРИЙ Б: Это был ПОСЛЕДНИЙ пользователь (updatedUserCount == 0) ---
                    // Он удалит ключ ТОЛЬКО если значение в словаре равно oldValue (счетчик == 1)
                    var entryToRemove = KeyValuePair.Create(noradId, oldValue);
                    if (_satelliteThreads.TryRemove(entryToRemove))
                    {
                        _logger.LogWarning($"На спутнике {noradId} осталось 0 пользователей. Поток удален.");
                        oldValue.cts.Cancel();

                        return;
                    }

                    // Если TryRemove не смог удалить (потому что кто-то успел подключиться и поднять счетчик),
                    // цикл while прокрутится заново, увидит актуальный counter и уйдет в Сценарий А
                }
            }
        }

        // Главный бесконечный цикл фонового процесса. Вызывается один раз при старте сервера
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("=== Фоновый воркер OrbitX успешно запущен ===");

            // Запоминаем токен сервера, чтобы связывать его с токенами спутников
            _serverStoppingToken = stoppingToken;
            // Цикл каждые 6 часов
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromHours(6));

            // Цикл получения и обновления данных
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    // Создаем стерильную Scoped-область
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        stoppingToken.ThrowIfCancellationRequested();
                        
                        var downloaderTLE = scope.ServiceProvider.GetRequiredService<SatelliteTLEDownloader>();
                        await downloaderTLE.GetTLEData(stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Метод корректно завершается при выключении сервера
            }

            _logger.LogInformation("=== Фоновый воркер OrbitX остановлен ===");
        }

        // ИЗОЛИРОВАННЫЙ, ПАРАЛЛЕЛЬНЫЙ ПОТОК РАСЧЕТА ДЛЯ КОНКРЕТНОГО ID СПУТНИКА
        private async Task StartSatelliteStreamingThread(int noradId, CancellationToken token, CancellationTokenSource cts)
        {
            _logger.LogInformation($"[Thread Engine] Запущен поток для спутника ID: {noradId}");

            try
            {
                // Бесконечный цикл
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        // Scope на каждом такте внутри цикла while, чтобы EF Core очищал соеднинение с базой PostgreSQL!
                        using var scope = _serviceProvider.CreateScope();
                        var sgpService = scope.ServiceProvider.GetRequiredService<ISatelliteSGPServices>();

                        // Получаем координаты на текущий такт
                        var data = await sgpService.GetSGPByID(noradId);

                        if (data != null)
                        {
                            Console.WriteLine($"Name:{data.Name}\nLat: {data.Latitude:F2}\nLon: {data.Longitude:F2}\nAlt: {data.Altitude:F2}");

                            // Пуш в SignalR
                            await _hubContext.Clients.Group($"Satellite_{noradId}").SendAsync("ReceivePosition", data, token);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"[Thread Engine] Пропущен тактовый сбой в потоке {noradId}: {ex.Message}");
                    }

                    // Обновление каждую секунду
                    await Task.Delay(1000, token);
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                cts.Dispose();
            }
        }
    }
}
