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
    public partial class SatelliteSGP4Service : ISatelliteSGPServices
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
            LogLaunchById();

            if (noradId < 0)
            {
                LogCancelNegativeNumber();
                return null;
            }

            var satelliteData = await _satelliteSGPRepository.GetTLEByID(noradId);

            if (satelliteData == null)
            {
                LogCancelNull(noradId);
                return null; // Безопасный выход!
            }

            LogProcessSGP4ById();

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

                LogSGP4DtoById(SGP4DataDTO.NoradId, SGP4DataDTO.Name, SGP4DataDTO.TLELine1, SGP4DataDTO.TLELine2, SGP4DataDTO.Longitude, SGP4DataDTO.Latitude, SGP4DataDTO.Altitude);

                return SGP4DataDTO;
            }
            catch (Exception ex)
            {
                LogFailureById(ex);
                return null;
            }
            
        }

        public async Task<SGP4DataDTO?> GetSGPByName(string satelliteName)
        {
            LogLaunchByName();

            if (string.IsNullOrEmpty(satelliteName))
            {
                LogCancelNameNull();
                return null;
            }

            var satelliteData = await _satelliteSGPRepository.GetTLEByName(satelliteName);

            if (satelliteData == null)
            {
                LogCancelNull(satelliteName);
                return null; // Безопасный выход!
            }

            LogProcessSGP4ByName();

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

                LogSGP4DtoByName(SGP4DataDTO.NoradId, SGP4DataDTO.Name, SGP4DataDTO.TLELine1, SGP4DataDTO.TLELine2, SGP4DataDTO.Longitude, SGP4DataDTO.Latitude, SGP4DataDTO.Altitude);

                return SGP4DataDTO;
            }
            catch (Exception ex)
            {
                LogFailureByName(ex);
                return null;
            }
            
        }
    }
}
