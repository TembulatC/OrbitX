using Microsoft.Extensions.Logging;

namespace Core.Modules.SGP4Data.Infrastructure.Repositories
{
    public partial class SatelliteSGPRepository
    {
        #region GetTLEByID

        [LoggerMessage(
            EventId = 701,
            Level = LogLevel.Information,
            Message = "Запуск получения данных спутника с ID {NoradId} из базы данных")]
        private partial void LogLaunchById(int noradId);

        [LoggerMessage(
            EventId = 702,
            Level = LogLevel.Warning,
            Message = "Отмена получения данных. ID спутника не может быть отрицательным")]
        private partial void LogNegativeNumber();

        [LoggerMessage(
            EventId = 703,
            Level = LogLevel.Information,
            Message = "Данные о спутнике успешно получены")]
        private partial void LogSuccessById();

        [LoggerMessage(
            EventId = 704,
            Level = LogLevel.Warning,
            Message = "Спутника с ID {NoradId} не существует в базе данных")]
        private partial void LogNotFoundById(int noradId);

        #endregion

        #region GetTLEByName

        [LoggerMessage(
            EventId = 801,
            Level = LogLevel.Information,
            Message = "Запуск получения данных спутника {SatelliteName} из базы данных")]
        private partial void LogLaunchByName(string satelliteName);

        [LoggerMessage(
            EventId = 802,
            Level = LogLevel.Warning,
            Message = "Отмена получения данных. Имя спутника не может быть пустым")]
        private partial void LogCancelNullByName();

        [LoggerMessage(
            EventId = 803,
            Level = LogLevel.Information,
            Message = "Данные о спутнике успешно получены")]
        private partial void LogSuccessByName();

        [LoggerMessage(
            EventId = 804,
            Level = LogLevel.Warning,
            Message = "Спутника {SatelliteName} не существует в базе данных")]
        private partial void LogNotFoundByName(string satelliteName);

        #endregion
    }
}
