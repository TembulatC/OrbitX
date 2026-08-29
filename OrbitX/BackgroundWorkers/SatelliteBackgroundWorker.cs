using Core.Modules.SGP4Data.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using OrbitX.BackgroundWorkers.Helper;
using OrbitX.SignalRHubs;
using System.Collections.Concurrent;

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
            // Связываем токен спутника с токеном сервера
            // Поток умрет или при вызове OnSatelliteUnwatched, или при выключении всего сервера
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_serverStoppingToken);

            // Флаг, чтобы запустить поток только в случае реального ДОБАВЛЕНИЯ нового спутника
            bool isNewSatelliteAdded = false;

            _satelliteThreads.AddOrUpdate(  
                noradId,
                (key) => 
                {
                    isNewSatelliteAdded = true;
                    return (linkedCts, 1); // Записываем токен и ставим счетчик в 1
                },
    
                (key, oldValue) => 
                {
                    // Спутник уже стримит данные в другом потоке. Новый токен не нужен — уничтожаем его.
                    linkedCts.Dispose();
                    // Возвращаем старый токен, но счетчик увеличиваем на 1
                    return (oldValue.cts, oldValue.counter + 1);
                }
            );

            // Запускаем независимый поток расчета ТОЛЬКО если это был первый запуск для этого noradId
            if (isNewSatelliteAdded)
            {
                _logger.LogInformation($"Инициализация сокета для спутника: {noradId}");

                _ = Task.Run(() => StartSatelliteStreamingThread(noradId, linkedCts.Token), linkedCts.Token);
            }
            else
            {
                _logger.LogInformation($"К сокету спутника {noradId} подключился новый пользователь");
            }
        }

        // Метод вызывается из SignalR при отключении
        public void OnSatelliteUnwatched(int noradId)
        {
            // Если этого спутника по какой-то причине нет в словаре — сразу выходим
            if (!_satelliteThreads.ContainsKey(noradId)) return;

            CancellationTokenSource? ctsToKill = null;

            _satelliteThreads.AddOrUpdate(
                noradId,
                // На случай непредвиденных сбоев
                (key) => (CancellationTokenSource.CreateLinkedTokenSource(_serverStoppingToken), 0),

                (key, oldValue) =>
                {
                    int updatedUserCount = Math.Max(0, oldValue.counter - 1);
                    _logger.LogWarning($"[Worker] Пользователь покинул спутник {key}. Осталось людей в потоке: {updatedUserCount}");

                    // Если на спутнике больше никого не осталось — готовим поток к уничтожению
                    if (updatedUserCount == 0)
                    {
                        ctsToKill = oldValue.cts;
                    }

                    return (oldValue.cts, updatedUserCount);
                }
            );

            if (ctsToKill != null)
            {
                // Удаляем запись из адресной книги, чтобы освободить место
                if (_satelliteThreads.TryRemove(noradId, out _))
                {
                    _logger.LogCritical($"[Worker]: На спутнике {noradId} осталось 0 пользователей. Поток расчетов удален.");

                    ctsToKill.Cancel(); // Мгновенно останавливаем бесконечный цикл внутри StartSatelliteStreamingThread
                    ctsToKill.Dispose(); // Чистим системные ресурсы токена
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
        private async Task StartSatelliteStreamingThread(int noradId, CancellationToken token)
        {
            _logger.LogInformation($"[Thread Engine] Запущен поток для спутника ID: {noradId}");

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
    }
}
