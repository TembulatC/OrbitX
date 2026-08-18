
using Core.Modules.SGP4Data.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using OrbitX.SignalRHubs;
using System.Collections.Concurrent;

namespace OrbitX.BackgroundWorkers
{
    public class SatelliteBackgroundWorker : BackgroundService
    {
        // Вызов провайдера для Scoped
        private readonly IServiceProvider _serviceProvider;
        // Адресная книга. ID спутника -> Токен отмены его потока данных
        private static readonly ConcurrentDictionary<int, CancellationTokenSource> _satelliteThreads = new();
        // Связываем с SignalR
        private readonly IHubContext<SignalRHub> _hubContext;
        // Логгер
        private readonly ILogger<SatelliteBackgroundWorker> _logger;
        // Ссылка на токен остановки всего сервера
        private CancellationToken _serverStoppingToken;

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

            if (_satelliteThreads.TryAdd(noradId, linkedCts))
            {
                _logger.LogInformation($"Инициализация стриминга данных для спутника: {noradId}");

                // Запускаем единожды независимый поток расчета
                _ = Task.Run(() => StartSatelliteStreamingThread(noradId, linkedCts.Token), linkedCts.Token);
            }
            else
            {
                // Если спутник уже есть и его поток активен, удаляем новый токен и оставляем тот же
                linkedCts.Dispose();
            }
        }

        // Метод вызывается из SignalR при отключении
        public void OnSatelliteUnwatched(int noradId)
        {
            // Если мы явно шлем сигнал, что сокетов больше нет, вытаскиваем токен из адресной книги и удаляем поток
            if (_satelliteThreads.TryRemove(noradId, out var linkedCts))
            {
                linkedCts.Cancel();
                linkedCts.Dispose();
            }
        }

        // Главный бесконечный цикл фонового процесса. Вызывается один раз при старте сервера
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Запоминаем токен сервера, чтобы связывать его с токенами спутников
            _serverStoppingToken = stoppingToken;

            _logger.LogInformation("=== Фоновый воркер OrbitX успешно запущен ===");

            // Просто держим воркер живым, пока сервер работает.
            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
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
            _logger.LogInformation($"[Thread Engine] Запущен кастомный параллельный поток для спутника ID: {noradId}");

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

            _logger.LogInformation($"[Thread Engine] Поток для спутника ID: {noradId} успешно удален и стерт из памяти.");
        }
    }
}
