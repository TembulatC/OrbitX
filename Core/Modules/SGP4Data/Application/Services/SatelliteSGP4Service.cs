using Core.Modules.SGP4Data.Application.DTOs;
using Core.Modules.SGP4Data.Application.Interfaces;
using Core.Modules.SGP4Data.Domain.Interfaces.Repositories;
using Core.Modules.SGP4Data.Domain.Interfaces.Services;
using Core.Modules.SGP4Data.Domain.Models;
using Core.Modules.TLEData.Domain.Models;
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
    public class SatelliteSGP4Service : ISatelliteSGPServices, ISGP
    {
        private readonly ISatelliteSGPRepository _satelliteSGPRepository;

        public SatelliteSGP4Service(ISatelliteSGPRepository satelliteSGPRepository)
        {
            _satelliteSGPRepository = satelliteSGPRepository;
        }

        public async Task<SatelliteTLE?> GetSGPByID(int noradId)
        {
            var satellite = await _satelliteSGPRepository.GetTLEByID(noradId);

            if (satellite == null) return null;

            return satellite;
        }

        public async Task<SatelliteTLE?> GetSGPByName(string satelliteName)
        {
            var satellite = await _satelliteSGPRepository.GetTLEByName(satelliteName);

            if (satellite == null) return null;

            return satellite;
        }

        public async Task<SGP4DataDTO> GetSGP(int noradId)
        {
            var satelliteData = await GetSGPByID(noradId);
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

        public async Task<SGP4DataDTO> GetSGP(string satelliteName)
        {
            var satelliteData = await GetSGPByName(satelliteName);
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
