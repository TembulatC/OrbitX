using Core.Modules.SGP4Data.Application.DTOs;
using Core.Modules.SGP4Data.Application.Interfaces;
using Core.Modules.SGP4Data.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using SGPdotNET.CoordinateSystem;
using SGPdotNET.Parsers;
using SGPdotNET.TLE;
using SGPdotNET.Observation;

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
                // Инициализируем объекты Omm для движка SGP4 
                var omm = new OmmData
                {
                    OmmVersion = "2.0", // Дефолтный стандарт CCSDS OMM
                    ObjectName = satelliteData.OBJECT_NAME,
                    ObjectID = satelliteData.OBJECT_ID,
                    NoradCatID = (uint)satelliteData.NORAD_CAT_ID, // Приводим int базы к uint библиотеки
                    ClassificationType = satelliteData.CLASSIFICATION_TYPE,
                    ElementSetNo = (uint)satelliteData.ELEMENT_SET_NO,
                    Epoch = DateTime.SpecifyKind(satelliteData.EPOCH, DateTimeKind.Utc), // Указываем UTC
                    MeanMotion = satelliteData.MEAN_MOTION,
                    Eccentricity = satelliteData.ECCENTRICITY,
                    Inclination = satelliteData.INCLINATION,
                    RAOfAscNode = satelliteData.RA_OF_ASC_NODE,
                    ArgOfPericenter = satelliteData.ARG_OF_PERICENTER,
                    MeanAnomaly = satelliteData.MEAN_ANOMALY,
                    EphemerisType = satelliteData.EPHEMERIS_TYPE,
                    BStar = satelliteData.BSTAR,
                    MeanMotionDot = satelliteData.MEAN_MOTION_DOT,
                    MeanMotionDDot = satelliteData.MEAN_MOTION_DDOT,
                    RevAtEpoch = (uint)satelliteData.REV_AT_EPOCH,
                    CenterName = "EARTH",
                    RefFrame = "TEME",
                    TimeSystem = "UTC",
                    MeanElementTheory = "SGP4"
                };

                var satellite = new Satellite(omm);

                // Расчет позиции ECI на текущее время UTC
                DateTime utcTime = DateTime.UtcNow;
                EciCoordinate eci = satellite.Predict(utcTime);

                // Переводим декартов вектор ECI в геодезические градусы геоида Земли WGS-84
                GeodeticCoordinate geoPosition = eci.ToGeodetic();

                SGP4DataDTO SGP4DataDTO = new SGP4DataDTO
                {
                    NoradId = satelliteData.NORAD_CAT_ID,
                    Name = satelliteData.OBJECT_NAME,
                    Longitude = geoPosition.Longitude.Degrees,
                    Latitude = geoPosition.Latitude.Degrees,
                    Altitude = geoPosition.Altitude,
                };

                LogSGP4DtoById(SGP4DataDTO.NoradId, SGP4DataDTO.Name, SGP4DataDTO.Longitude, SGP4DataDTO.Latitude, SGP4DataDTO.Altitude);

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
                // Инициализируем объекты Omm для движка SGP4 
                var omm = new OmmData
                {
                    OmmVersion = "2.0", // Дефолтный стандарт CCSDS OMM
                    ObjectName = satelliteData.OBJECT_NAME,
                    ObjectID = satelliteData.OBJECT_ID,
                    NoradCatID = (uint)satelliteData.NORAD_CAT_ID, // Приводим int базы к uint библиотеки
                    ClassificationType = satelliteData.CLASSIFICATION_TYPE,
                    ElementSetNo = (uint)satelliteData.ELEMENT_SET_NO,
                    Epoch = DateTime.SpecifyKind(satelliteData.EPOCH, DateTimeKind.Utc), // Указываем UTC
                    MeanMotion = satelliteData.MEAN_MOTION,
                    Eccentricity = satelliteData.ECCENTRICITY,
                    Inclination = satelliteData.INCLINATION,
                    RAOfAscNode = satelliteData.RA_OF_ASC_NODE,
                    ArgOfPericenter = satelliteData.ARG_OF_PERICENTER,
                    MeanAnomaly = satelliteData.MEAN_ANOMALY,
                    EphemerisType = satelliteData.EPHEMERIS_TYPE,
                    BStar = satelliteData.BSTAR,
                    MeanMotionDot = satelliteData.MEAN_MOTION_DOT,
                    MeanMotionDDot = satelliteData.MEAN_MOTION_DDOT,
                    RevAtEpoch = (uint)satelliteData.REV_AT_EPOCH,
                    CenterName = "EARTH",
                    RefFrame = "TEME",
                    TimeSystem = "UTC",
                    MeanElementTheory = "SGP4"
                };

                var satellite = new Satellite(omm);

                // Расчет позиции ECI на текущее время UTC
                DateTime utcTime = DateTime.UtcNow;
                EciCoordinate eci = satellite.Predict(utcTime);

                // Переводим декартов вектор ECI в геодезические градусы геоида Земли WGS-84
                GeodeticCoordinate geoPosition = eci.ToGeodetic();

                SGP4DataDTO SGP4DataDTO = new SGP4DataDTO
                {
                    NoradId = satelliteData.NORAD_CAT_ID,
                    Name = satelliteData.OBJECT_NAME,
                    Longitude = geoPosition.Longitude.Degrees,
                    Latitude = geoPosition.Latitude.Degrees,
                    Altitude = geoPosition.Altitude,
                };

                LogSGP4DtoByName(SGP4DataDTO.NoradId, SGP4DataDTO.Name, SGP4DataDTO.Longitude, SGP4DataDTO.Latitude, SGP4DataDTO.Altitude);

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
