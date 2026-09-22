using Core.Modules.SGP4Data.Domain.Interfaces;
using Core.Modules.SGP4Data.Domain.Models;
using Core.Modules.SGP4Data.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Core.Modules.SGP4Data.Infrastructure.Repositories
{
    public partial class SatelliteSGPRepository : ISatelliteSGPRepository
    {
        private readonly SGP4DBContext _dbContext;
        private readonly ILogger<SatelliteSGPRepository> _logger;
        private readonly IMemoryCache _cache; // Кэширование

        public SatelliteSGPRepository(SGP4DBContext dbContext, ILogger<SatelliteSGPRepository> logger, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Satellite?> GetTLEByID(int noradId)
        {
            LogLaunchById(noradId);

            if (noradId < 0)
            {
                LogNegativeNumber();
                return null;
            }

            // Формируем уникальный ключ для кеша
            string cacheKey = $"omm:{noradId}";

            // Проверяем, есть ли уже данные в кеше
            if (!_cache.TryGetValue(cacheKey, out Satellite? satelliteTLE))
            {
                // Если в кеше нет — ОДИН раз идем в базу данных
                satelliteTLE = await _dbContext.SatellitesTLE.FindAsync(noradId);

                if (satelliteTLE != null)
                {
                    LogSuccessById();

                    // Сохраняем в кеш 
                    var cacheOptions = new MemoryCacheEntryOptions()
                     // Спутник удалится из памяти через 2 часа
                    .SetAbsoluteExpiration(TimeSpan.FromHours(2))
                    // Если этот спутник никто не смотрел 15 минут — выкидываем его, чтобы не забивать ОЗУ сервера
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15))
                    // Задаем высокий приоритет, чтобы сборщик мусора (GC) не снес его принудительно
                    .SetPriority(CacheItemPriority.High);

                    // Записываем OMM-сущность из базы в оперативную память сервера
                    _cache.Set(cacheKey, satelliteTLE, cacheOptions);
                }
                else LogNotFoundById(noradId);
            }

            return satelliteTLE;
        }

        public async Task<Satellite?> GetTLEByName(string satelliteName)
        {
            LogLaunchByName(satelliteName);

            if (string.IsNullOrEmpty(satelliteName))
            {
                LogCancelNullByName();
                return null;
            }

            var satelliteTLE = await _dbContext.SatellitesTLE.AsNoTracking().FirstOrDefaultAsync(s => s.OBJECT_NAME == satelliteName);

            if (satelliteTLE != null) LogSuccessByName();
            else LogNotFoundByName(satelliteName);

            return satelliteTLE;
        }
    }
}
