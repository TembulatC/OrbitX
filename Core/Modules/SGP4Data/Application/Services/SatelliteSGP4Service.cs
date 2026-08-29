using Core.Modules.SGP4Data.Application.DTOs;
using Core.Modules.SGP4Data.Application.Interfaces;
using Core.Modules.SGP4Data.Domain.Interfaces;
using Core.Modules.SGP4Data.Domain.Models;
using Core.Modules.TLEData.Domain.Models;
using Microsoft.Extensions.Logging;
using SGPdotNET.CoordinateSystem;
using SGPdotNET.Observation;
using SGPdotNET.Propagation;
using SGPdotNET.TLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SGP4Data.Application.Services
{
    public class SatelliteSGP4Service : ISatelliteSGPServices
    {
        private readonly ISatelliteSGPRepository _satelliteSGPRepository;
        private readonly ILogger<SatelliteSGP4Service> _logger;

        public SatelliteSGP4Service(ISatelliteSGPRepository satelliteSGPRepository, ILogger<SatelliteSGP4Service> logger)
        {
            _satelliteSGPRepository = satelliteSGPRepository;
            _logger = logger;
        }

        public async Task<SGP4DataDTO?> GetSGPByID(int noradId)
        {
            _logger.LogInformation("Цикл запущен");

            var satelliteData = await _satelliteSGPRepository.GetTLEByID(noradId);

            if (satelliteData == null)
            {
                _logger.LogWarning($"Отмена цикла. Данные о спутнике с ID {noradId} не найдены");
                return null; // Безопасный выход!
            }

            _logger.LogInformation("Обработка данных в SGP4 для получения координат спутника");

            try
            {
                var tle = new Tle(satelliteData.Name, satelliteData.TLELine1, satelliteData.TLELine2); // Инициализируем объекты TLE для движка SGP4          
                var satellite = new SGPdotNET.Observation.Satellite(tle); // Инициализация спутника           

                // Расчет позиции ECI на текущее время UTC
                DateTime utcTime = DateTime.UtcNow;
                EciCoordinate eci = satellite.Predict(utcTime);

                // Переводим декартов вектор ECI в геодезические градусы геоида Земли WGS-84
                GeodeticCoordinate geoPosition = eci.ToGeodetic();

                SGP4DataDTO SGP4DataDTO = new SGP4DataDTO
                {
                    NoradId = satelliteData.NoradId,
                    Name = satelliteData.Name,
                    TLELine1 = satelliteData.TLELine1,
                    TLELine2 = satelliteData.TLELine2,
                    Longitude = geoPosition.Longitude.Degrees,
                    Latitude = geoPosition.Latitude.Degrees,
                    Altitude = geoPosition.Altitude,
                };

                _logger.LogInformation($"Данные спутника успешно получены:\n" +
                    $"NoradId - {SGP4DataDTO.NoradId}\n" +
                    $"Name - {SGP4DataDTO.Name}\n" +
                    $"TLELine1 - {SGP4DataDTO.TLELine1}\n" +
                    $"TLELine2 - {SGP4DataDTO.TLELine2}\n" +
                    $"Longtitude - {SGP4DataDTO.Longitude}\n" +
                    $"Latitude - {SGP4DataDTO.Latitude}\n" +
                    $"Altitude - {SGP4DataDTO.Altitude}");

                return SGP4DataDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Сбой математического расчета SGP4");
                return null;
            }
            
        }

        public async Task<SGP4DataDTO?> GetSGPByName(string satelliteName)
        {
            var satelliteData = await _satelliteSGPRepository.GetTLEByName(satelliteName);
            if (satelliteData == null) return null; // Безопасный выход!

            var tle = new Tle(satelliteData.Name, satelliteData.TLELine1, satelliteData.TLELine2); // Инициализируем объекты TLE для движка SGP4          
            var satellite = new SGPdotNET.Observation.Satellite(tle); // Инициализация спутника           

            // Расчет позиции ECI на текущее время UTC
            DateTime utcTime = DateTime.UtcNow;
            EciCoordinate eci = satellite.Predict(utcTime);

            // Переводим декартов вектор ECI в геодезические градусы геоида Земли WGS-84
            GeodeticCoordinate geoPosition = eci.ToGeodetic();

            SGP4DataDTO sGP4DataDTO = new SGP4DataDTO
            {
                NoradId = satelliteData.NoradId,
                Name = satelliteData.Name,
                TLELine1 = satelliteData.TLELine1,
                TLELine2 = satelliteData.TLELine2,
                Longitude = geoPosition.Longitude.Degrees,
                Latitude = geoPosition.Latitude.Degrees,
                Altitude = geoPosition.Altitude,
            };

            return sGP4DataDTO;
        }
    }
}
