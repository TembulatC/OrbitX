namespace OrbitX.SignalRHubs
{
    public partial class SignalRHub
    {
        [LoggerMessage(
            EventId = 2001,
            Level = LogLevel.Warning,
            Message = "Клиент SignalR отключен аварийно. ConnectionId: {ConnectionId}. Ошибка: {ExceptionMessage}")]
        private partial void LogClientAbruptDisconnect(string connectionId, string exceptionMessage);
    }
}
