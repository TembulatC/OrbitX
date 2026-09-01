using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SGP4Data.Application.Services
{
    public partial class SatelliteSGP4Service
    {
        #region GetSGPByID

        [LoggerMessage(
            EventId = 501,
            Level = LogLevel.Information,
            Message = "Цикл запущен")]
        private partial void LogLaunchById();

        [LoggerMessage(
            EventId = 502,
            Level = LogLevel.Warning,
            Message = "Отмена цикла. Передан отрицательный NoradId")]
        private partial void LogCancelNegativeNumber();

        [LoggerMessage(
            EventId = 503,
            Level = LogLevel.Warning,
            Message = "Отмена цикла. Данные о спутнике с ID {NoradId} не найдены")]
        private partial void LogCancelNull(int noradId);

        [LoggerMessage(
            EventId = 504,
            Level = LogLevel.Information,
            Message = "Обработка данных в SGP4 для получения координат спутника")]
        private partial void LogProcessSGP4ById();

        [LoggerMessage(
            EventId = 505,
            Level = LogLevel.Information,
            Message = "Данные спутника:\n" +
                    "NoradId - {NoradId}\n" +
                    "Name - {Name}\n" +
                    "TLELine1 - {TLELine1}\n" +
                    "TLELine2 - {TLELine2}\n" +
                    "Longtitude - {Longitude}\n" +
                    "Latitude - {Latitude}\n" +
                    "Altitude - {Altitude}")]
        private partial void LogSGP4DtoById(int noradId, string name, string tleLine1, string tleLine2, double longitude, double latitude, double altitude);

        [LoggerMessage(
            EventId = 506,
            Level = LogLevel.Error,
            Message = "Сбой математического расчета SGP4")]
        private partial void LogFailureById(Exception ex);

        #endregion

        #region GetSGPByName

        [LoggerMessage(
            EventId = 601,
            Level = LogLevel.Information,
            Message = "Цикл запущен")]
        private partial void LogLaunchByName();

        [LoggerMessage(
            EventId = 602,
            Level = LogLevel.Warning,
            Message = "Отмена цикла. Пришло пустое имя спутника")]
        private partial void LogCancelNameNull();

        [LoggerMessage(
            EventId = 603,
            Level = LogLevel.Warning,
            Message = "Отмена цикла. Данные о спутнике {SatelliteName} не найдены")]
        private partial void LogCancelNull(string satelliteName);

        [LoggerMessage(
            EventId = 604,
            Level = LogLevel.Information,
            Message = "Обработка данных в SGP4 для получения координат спутника")]
        private partial void LogProcessSGP4ByName();

        [LoggerMessage(
            EventId = 605,
            Level = LogLevel.Information,
            Message = "Данные спутника:\n" +
                    "NoradId - {NoradId}\n" +
                    "Name - {Name}\n" +
                    "TLELine1 - {TLELine1}\n" +
                    "TLELine2 - {TLELine2}\n" +
                    "Longtitude - {Longitude}\n" +
                    "Latitude - {Latitude}\n" +
                    "Altitude - {Altitude}")]
        private partial void LogSGP4DtoByName(int noradId, string name, string tleLine1, string tleLine2, double longitude, double latitude, double altitude);

        [LoggerMessage(
            EventId = 606,
            Level = LogLevel.Error,
            Message = "Сбой математического расчета SGP4")]
        private partial void LogFailureByName(Exception ex);

        #endregion
    }
}
