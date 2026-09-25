namespace OrbitX.BackgroundWorkers
{
    public partial class SatelliteBackgroundWorker
    {
        [LoggerMessage(
            EventId = 3001,
            Level = LogLevel.Information,
            Message = "К спутнику {NoradId} подключился первый пользователь. Токен создан")]
        private partial void LogAddToken(int noradId);

        [LoggerMessage(
            EventId = 3002,
            Level = LogLevel.Information,
            Message = "Запущен поток для спутника {NoradId}")]
        private partial void LogAddTask(int noradId);

        [LoggerMessage(
            EventId = 3003,
            Level = LogLevel.Warning,
            Message = "Сбой в такте потока спутника {NoradId}: {Message}")]
        private partial void LogException(int noradId, string message);

        [LoggerMessage(
            EventId = 3004,
            Level = LogLevel.Information,
            Message = "Поток спутника {NoradId} завершает работу по требованию CancellationToken")]
        private partial void LogOperationCanceledException(int noradId);

        [LoggerMessage(
            EventId = 3005,
            Level = LogLevel.Information,
            Message = "Поток спутника {NoradId} завершает работу самостоятельно между шагами по требованию CancellationToken")]
        private partial void LogOperationCanceled(int noradId);

        [LoggerMessage(
            EventId = 3006,
            Level = LogLevel.Information,
            Message = "На спутнике {NoradId} осталось 0 пользователей. Отправлено требование CancellationToken на отмену потока")]
        private partial void LogCancelTask(int noradId);

        [LoggerMessage(
            EventId = 3007,
            Level = LogLevel.Information,
            Message = "Поток спутника {NoradId} удален")]
        private partial void LogDisposeTask(int noradId);

        [LoggerMessage(
            EventId = 3008,
            Level = LogLevel.Information,
            Message = "Фоновые процессы OrbitX запущены")]
        private partial void LogLaunchWorker();

        [LoggerMessage(
            EventId = 3009,
            Level = LogLevel.Information,
            Message = "Фоновые процессы OrbitX остановлены")]
        private partial void LogStopWorker();

        [LoggerMessage(
            EventId = 3010,
            Level = LogLevel.Information,
            Message = "Через 2 часа будет запущен цикл обновления данных спутников")]
        private partial void Log2HoursСycle();

        [LoggerMessage(
            EventId = 3011,
            Level = LogLevel.Information,
            Message = "Цикл обновления данных спутников запущен")]
        private partial void LogLaunchСycle();

        [LoggerMessage(
            EventId = 3012,
            Level = LogLevel.Information,
            Message = "Цикл обновления данных спутников окончен. Следующий через 2 часа")]
        private partial void LogEndСycle();
    }
}


namespace OrbitX.BackgroundWorkers.Helper
{
    public partial class SatelliteOMMDownloader
    {
        [LoggerMessage(
            EventId = 4001,
            Level = LogLevel.Error,
            Message = "(HTTP {StatusCode}) Доступ временно заблокирован из-за слишком частых запросов. Цикл продолжится через 3 часа с URL: {Url}")]
        private partial void Log403StatusCode(int statusCode, string url);

        [LoggerMessage(
           EventId = 4002,
           Level = LogLevel.Warning,
           Message = "(HTTP {StatusCode}) Страницы к которой был HTTP-запрос не существует. Переходим к следующей категории")]
        private partial void Log404StatusCode(int statusCode);

        [LoggerMessage(
           EventId = 4003,
           Level = LogLevel.Error,
           Message = "(HTTP {StatusCode}) На внешнем сервере запросов произошел сбой. Цикл продолжится через 10 минут с URL: {Url}")]
        private partial void Log500StatusCode(int statusCode, string url);

        [LoggerMessage(
           EventId = 4004,
           Level = LogLevel.Warning,
           Message = "(HTTP {StatusCode}) Внешний сервер запросов временно недоступен. Цикл продолжится через 30 минут с URL: {Url}")]
        private partial void Log503StatusCode(int statusCode, string url);

        [LoggerMessage(
           EventId = 4005,
           Level = LogLevel.Error,
           Message = "(HTTP {StatusCode}) Сетевой запрос завершился с неизвестной ошибкой: {ReasonPhrase}")]
        private partial void LogUnknownError(int statusCode, string? reasonPhrase);

        [LoggerMessage(
           EventId = 4006,
           Level = LogLevel.Information,
           Message = "{RequestCount} запрос через {Seconds} секунд(ы). URL: {Url}")]
        private partial void LogHttpRequest(int requestCount, int seconds, string url);

        [LoggerMessage(
           EventId = 4007,
           Level = LogLevel.Information,
           Message = "Запрос прошел успешно")]
        private partial void LogSuccess();

        [LoggerMessage(
           EventId = 4008,
           Level = LogLevel.Warning,
           Message = "Тело HTTP-запроса пришло пустым переходим к следующему")]
        private partial void LogHTTPBodyNull();

        [LoggerMessage(
           EventId = 4009,
           Level = LogLevel.Warning,
           Message = "Парсер вернул пустой список спутников. Переходим к следующему запросу")]
        private partial void LogParseBodyNull();

        [LoggerMessage(
           EventId = 4010,
           Level = LogLevel.Warning,
           Message = "Неверный запрос. Ссылка некорректна/спутников такой категории не существует. Переходим к следующему запросу")]
        private partial void LogInvalid();

        [LoggerMessage(
           EventId = 4011,
           Level = LogLevel.Error,
           Message = "Неизвестная сетевая ошибка при HTTP-запросе. Переходим к следующему запросу")]
        private partial void LogUnknownHttpError(HttpRequestException ex);

        [LoggerMessage(
           EventId = 4012,
           Level = LogLevel.Error,
           Message = "Неизвестная ошибка. Переходим к следующему запросу")]
        private partial void LogOtherError(Exception ex);

        [LoggerMessage(
           EventId = 4012,
           Level = LogLevel.Critical,
           Message = "Останавливаем весь цикл обхода")]
        private partial void LogCritical403Error(Exception ex);
    }
}