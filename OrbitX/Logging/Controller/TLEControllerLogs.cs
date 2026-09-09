using Microsoft.Extensions.Logging;

namespace OrbitX.Controllers
{
    public partial class TLEController
    {
        #region AddTLEData

        [LoggerMessage(
            EventId = 1001,
            Level = LogLevel.Information,
            Message = "Запуск операции обновления данных спутников категории - {SatellitesCategory}")]
        private partial void LogLaunchAdd(string satellitesCategory);

        [LoggerMessage(
            EventId = 1002,
            Level = LogLevel.Warning,
            Message = "Отмена операции. Передано пустое или некорректное имя категории спутников")]
        private partial void LogCancelOperation();

        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Information,
            Message = "Результат операции: Успех")]
        private partial void LogSuccessAdd();

        #endregion

        #region GetSGP4DataById

        [LoggerMessage(
            EventId = 1101,
            Level = LogLevel.Information,
            Message = "Запуск операции получения данных о спутнике с ID {NoradId} на текущий момент")]
        private partial void LogLaunchGetById(int noradId);

        [LoggerMessage(
            EventId = 1102,
            Level = LogLevel.Warning,
            Message = "Отмена операции")]
        private partial void LogCancelNullById();

        [LoggerMessage(
            EventId = 1103,
            Level = LogLevel.Warning,
            Message = "Отмена операции. Передан отрицательный NoradId")]
        private partial void LogCancelNegativeNumber();

        [LoggerMessage(
            EventId = 1104,
            Level = LogLevel.Information,
            Message = "Результат операции: Успех")]
        private partial void LogSuccessGetById();

        #endregion

        #region GetSGP4DataByName

        [LoggerMessage(
            EventId = 1201,
            Level = LogLevel.Information,
            Message = "Запуск операции получения данных о спутнике {SatelliteName} на текущий момент")]
        private partial void LogLaunchGetByName(string satelliteName);

        [LoggerMessage(
            EventId = 1202,
            Level = LogLevel.Warning,
            Message = "Отмена операции. Передано пустое или некорректное имя спутника")]
        private partial void LogCancelOperationName();

        [LoggerMessage(
            EventId = 1203,
            Level = LogLevel.Warning,
            Message = "Отмена операции")]
        private partial void LogCancelNullByName();

        [LoggerMessage(
            EventId = 1204,
            Level = LogLevel.Information,
            Message = "Результат операции: Успех")]
        private partial void LogSuccessGetByName();

        #endregion
    }
}
