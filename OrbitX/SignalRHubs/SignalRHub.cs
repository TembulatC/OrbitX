using Microsoft.AspNetCore.SignalR;
using OrbitX.BackgroundWorkers;

namespace OrbitX.SignalRHubs
{
    public class SignalRHub : Hub
    {
        private readonly ILogger<SignalRHub> _logger;
        private readonly SatelliteBackgroundWorker _worker;

        public SignalRHub(ILogger<SignalRHub> logger, SatelliteBackgroundWorker worker)
        {
            _logger = logger;
            _worker = worker;
        }

        // Этот метод автоматически вызовет JavaScript, когда пользователь зайдет на страницу /{id_satellite}
        public async Task WatchSatellite(int noradId)
        {
            // Веерный пуш: Сажаем сокет пользователя в изолированную комнату (Группу) этого спутника
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Satellite_{noradId}");
            _logger.LogInformation($"[SignalR] Пользователь {Context.ConnectionId} подключился к группе спутника - {noradId}");

            // Кладем ID спутника в словарь текущего соединения
            Context.Items["ActiveSatelliteId"] = noradId;

            // Сигнализируем нашему фоновому воркеру, чтобы он проверил/запустил поток расчета
            _worker.OnSatelliteWatched(noradId);
        }

        // Вызывается автоматически, когда сокет пользователя успешно открылся
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        // Вызывается автоматически, когда вкладка закрыта или оборвался интернет
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.Items.TryGetValue("ActiveSatelliteId", out var cachedId) && cachedId is int noradId)
            {
                // Убираем сокет из группы
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Satellite_{noradId}");
                _logger.LogInformation($"[SignalR] Пользователь {Context.ConnectionId} покинул группу спутника - {noradId}");

                // Сигнализируем воркеру, чтобы он проверил, не пора ли тушить параллельный поток
                _worker.OnSatelliteUnwatched(noradId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
