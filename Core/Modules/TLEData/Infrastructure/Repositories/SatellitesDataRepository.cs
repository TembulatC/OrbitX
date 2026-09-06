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

        public async Task AddTLEData(List<Satellite> tle, string satellitesCategory)
        {
            LogLaunch();

            if (tle == null || tle.Count <= 0)
            {
                LogCancellationAddData();
                return;
            }

            // Берем ID только тех спутников, которые пришли в этой конкретной категории
            var incomingIds = tle.Select(t => t.NoradId).ToList();

            // Тянем из базы ТОЛЬКО те спутники, которые мы хотим обновить
            var existingSatellites = await _dbContext.Satellites
                .Where(s => incomingIds.Contains(s.NoradId))
                .ToDictionaryAsync(s => s.NoradId); // Переводим в Dictionary для быстрого поиска

            foreach (Satellite satelliteTle in tle)
            {
                // Ищем в памяти по ID
                if (existingSatellites.TryGetValue(satelliteTle.NoradId, out var existing))
                {
                    // Спутник найден — обновляем свойства
                    existing.Name = satelliteTle.Name;
                    existing.TLELine1 = satelliteTle.TLELine1;
                    existing.TLELine2 = satelliteTle.TLELine2;
                    existing.Epoch = satelliteTle.Epoch;
                    existing.UpdatedAt = satelliteTle.UpdatedAt;
                    existing.Category = satellitesCategory;
                }
                else
                {
                    _dbContext.Satellites.Add(satelliteTle);
                }
            }

            // Сохраняем всё одним мощным батчем
            await _dbContext.SaveChangesAsync();
            LogUpdateData();
        }

        public async Task AddTLEData(List<Satellite> tle, string satellitesCategory, CancellationToken cancellationToken)
        {
            if (tle == null || tle.Count <= 0)
            {
                return;
            }

            // Берем ID только тех спутников, которые пришли в этой конкретной категории
            var incomingIds = tle.Select(t => t.NoradId).ToList();

            // Тянем из базы ТОЛЬКО те спутники, которые мы хотим обновить
            var existingSatellites = await _dbContext.Satellites
                .Where(s => incomingIds.Contains(s.NoradId))
                .ToDictionaryAsync(s => s.NoradId, cancellationToken); // Переводим в Dictionary для быстрого поиска

            foreach (Satellite satelliteTle in tle)
            {
                // Ищем в памяти по ID
                if (existingSatellites.TryGetValue(satelliteTle.NoradId, out var existing))
                {
                    // Спутник найден — обновляем свойства
                    existing.Name = satelliteTle.Name;
                    existing.TLELine1 = satelliteTle.TLELine1;
                    existing.TLELine2 = satelliteTle.TLELine2;
                    existing.Epoch = satelliteTle.Epoch;
                    existing.UpdatedAt = satelliteTle.UpdatedAt;
                    existing.Category = satellitesCategory;
                }
                else
                {
                    _dbContext.Satellites.Add(satelliteTle);
                }
            }

            // Сохраняем всё одним мощным батчем
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
