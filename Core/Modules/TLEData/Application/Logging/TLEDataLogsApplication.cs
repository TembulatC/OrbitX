using Microsoft.Extensions.Logging;

namespace Core.Modules.TLEData.Application.Services
{
    public partial class SatellitesDataService
    {
        [LoggerMessage(
            EventId = 101,
            Level = LogLevel.Information,
            Message = "Цикл запущен")]
        private partial void LogLaunch();

        [LoggerMessage(
            EventId = 102,
            Level = LogLevel.Warning,
            Message = "Отмена запуска парсера. Строка данных пришла пустой")]
        private partial void LogCancellationParser();

        [LoggerMessage(
            EventId = 103,
            Level = LogLevel.Warning,
            Message = "Отмена запуска добавления данных в базу данных. Список спутников вернулся пустым")]
        private partial void LogCancellationDB();

        [LoggerMessage(
            EventId = 104,
            Level = LogLevel.Information,
            Message = "Цикл окончен")]
        private partial void LogEnding();
    }

    public partial class SatellitesParserService
    {
        [LoggerMessage(
            EventId = 201,
            Level = LogLevel.Information,
            Message = "Запуск парсера")]
        private partial void LogLaunch();

        [LoggerMessage(
            EventId = 202,
            Level = LogLevel.Warning,
            Message = "Отмена обработки парсером. Строка TLE данных пришла пустой")]
        private partial void LogCancellationParserProcessing();

        [LoggerMessage(
            EventId = 203,
            Level = LogLevel.Information,
            Message = "Данные спутников отформатированы")]
        private partial void LogParserFormatting();
    }
}
