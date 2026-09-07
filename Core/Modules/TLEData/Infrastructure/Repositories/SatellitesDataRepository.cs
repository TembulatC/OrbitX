using Core.Modules.TLEData.Domain.Interfaces;
using Core.Modules.TLEData.Domain.Models;
using Core.Modules.TLEData.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Modules.TLEData.Infrastructure.Repositories
{
    public partial class SatellitesDataRepository : ISatellitesDataRepository
    {
        private readonly TLEDBContext _dbContext;
        private readonly ILogger<SatellitesDataRepository> _logger;

        public SatellitesDataRepository(TLEDBContext dbContext, ILogger<SatellitesDataRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task AddTLEData(List<Satellite> omm, string satellitesCategory)
        {
            LogLaunch();

            if (omm == null || omm.Count <= 0)
            {
                LogCancellationAddData();
                return;
            }

            // Берем ID только тех спутников, которые пришли в этой конкретной категории
            var incomingIds = omm.Select(t => t.NORAD_CAT_ID).ToList();

            // Тянем из базы ТОЛЬКО те спутники, которые мы хотим обновить
            var existingSatellites = await _dbContext.Satellites
                .Where(s => incomingIds.Contains(s.NORAD_CAT_ID))
                .ToDictionaryAsync(s => s.NORAD_CAT_ID); // Переводим в Dictionary для быстрого поиска

            foreach (Satellite satelliteOmm in omm)
            {
                // Ищем в памяти по ID
                if (existingSatellites.TryGetValue(satelliteOmm.NORAD_CAT_ID, out var existing))
                {
                    // Спутник найден — обновляем свойства
                    existing.OBJECT_NAME = satelliteOmm.OBJECT_NAME;
                    existing.OBJECT_ID = satelliteOmm.OBJECT_ID;
                    existing.MEAN_MOTION = satelliteOmm.MEAN_MOTION;
                    existing.ECCENTRICITY = satelliteOmm.ECCENTRICITY;
                    existing.INCLINATION = satelliteOmm.INCLINATION;
                    existing.RA_OF_ASC_NODE = satelliteOmm.RA_OF_ASC_NODE;
                    existing.ARG_OF_PERICENTER = satelliteOmm.ARG_OF_PERICENTER;
                    existing.MEAN_ANOMALY = satelliteOmm.MEAN_ANOMALY;
                    existing.EPHEMERIS_TYPE = satelliteOmm.EPHEMERIS_TYPE;
                    existing.CLASSIFICATION_TYPE = satelliteOmm.CLASSIFICATION_TYPE;
                    existing.ELEMENT_SET_NO = satelliteOmm.ELEMENT_SET_NO;
                    existing.REV_AT_EPOCH = satelliteOmm.REV_AT_EPOCH;
                    existing.BSTAR = satelliteOmm.BSTAR;
                    existing.MEAN_MOTION_DOT = satelliteOmm.MEAN_MOTION_DOT;
                    existing.MEAN_MOTION_DDOT = satelliteOmm.MEAN_MOTION_DDOT;
                    existing.EPOCH = satelliteOmm.EPOCH;
                    existing.UpdatedAt = satelliteOmm.UpdatedAt;
                    existing.Category = satellitesCategory;
                }
                else
                {
                    _dbContext.Satellites.Add(satelliteOmm);
                }
            }

            // Сохраняем всё одним мощным батчем
            await _dbContext.SaveChangesAsync();
            LogUpdateData();
        }

        public async Task AddTLEData(List<Satellite> omm, string satellitesCategory, CancellationToken cancellationToken)
        {
            LogLaunchCts();

            if (omm == null || omm.Count <= 0)
            {
                LogCancellationAddDataCts();
                return;
            }

            // Берем ID только тех спутников, которые пришли в этой конкретной категории
            var incomingIds = omm.Select(t => t.NORAD_CAT_ID).ToList();

            // Тянем из базы ТОЛЬКО те спутники, которые мы хотим обновить
            var existingSatellites = await _dbContext.Satellites
                .Where(s => incomingIds.Contains(s.NORAD_CAT_ID))
                .ToDictionaryAsync(s => s.NORAD_CAT_ID, cancellationToken); // Переводим в Dictionary для быстрого поиска

            foreach (Satellite satelliteOmm in omm)
            {
                // Ищем в памяти по ID
                if (existingSatellites.TryGetValue(satelliteOmm.NORAD_CAT_ID, out var existing))
                {
                    // Спутник найден — обновляем свойства
                    existing.OBJECT_NAME = satelliteOmm.OBJECT_NAME;
                    existing.OBJECT_ID = satelliteOmm.OBJECT_ID;
                    existing.MEAN_MOTION = satelliteOmm.MEAN_MOTION;
                    existing.ECCENTRICITY = satelliteOmm.ECCENTRICITY;
                    existing.INCLINATION = satelliteOmm.INCLINATION;
                    existing.RA_OF_ASC_NODE = satelliteOmm.RA_OF_ASC_NODE;
                    existing.ARG_OF_PERICENTER = satelliteOmm.ARG_OF_PERICENTER;
                    existing.MEAN_ANOMALY = satelliteOmm.MEAN_ANOMALY;
                    existing.EPHEMERIS_TYPE = satelliteOmm.EPHEMERIS_TYPE;
                    existing.CLASSIFICATION_TYPE = satelliteOmm.CLASSIFICATION_TYPE;
                    existing.ELEMENT_SET_NO = satelliteOmm.ELEMENT_SET_NO;
                    existing.REV_AT_EPOCH = satelliteOmm.REV_AT_EPOCH;
                    existing.BSTAR = satelliteOmm.BSTAR;
                    existing.MEAN_MOTION_DOT = satelliteOmm.MEAN_MOTION_DOT;
                    existing.MEAN_MOTION_DDOT = satelliteOmm.MEAN_MOTION_DDOT;
                    existing.EPOCH = satelliteOmm.EPOCH;
                    existing.UpdatedAt = satelliteOmm.UpdatedAt;
                    existing.Category = satellitesCategory;
                }
                else
                {
                    _dbContext.Satellites.Add(satelliteOmm);
                }
            }

            // Сохраняем всё одним мощным батчем
            await _dbContext.SaveChangesAsync(cancellationToken);
            LogUpdateDataCts();
        }
    }
}
