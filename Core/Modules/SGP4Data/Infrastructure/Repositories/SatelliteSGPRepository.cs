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

        public async Task<Satellite?> GetOMMByID(int noradId)
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
            if (!_cache.TryGetValue(cacheKey, out Satellite? satelliteOMM))
            {
                // Если в кеше нет — один раз идем в базу данных
                satelliteOMM = await _dbContext.SatellitesTLE.FindAsync(noradId);

                if (satelliteOMM != null)
                {
                    LogSuccessById();

                    // Сохраняем в кеш 
                    var cacheOptions = new MemoryCacheEntryOptions()
                    // Спутник удалится из памяти через 30 минут
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
                    // Если этот спутник никто не смотрел 15 минут — выкидываем его, чтобы не забивать ОЗУ сервера
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15))
                    // Задаем высокий приоритет, чтобы сборщик мусора (GC) не снес его принудительно
                    .SetPriority(CacheItemPriority.High);

                    // Записываем OMM-сущность из базы в оперативную память сервера
                    _cache.Set(cacheKey, satelliteOMM, cacheOptions);
                }
                else
                {
                    // Сохраняем в короткий кеш для защиты от спама поиска несуществующих спутников
                    var cacheOptions = new MemoryCacheEntryOptions()
                    // Спутник удалится из памяти через 30 секунд
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

                    // Записываем OMM-сущность из базы в оперативную память сервера
                    _cache.Set(cacheKey, satelliteOMM, cacheOptions);

                    LogNotFoundById(noradId);
                }           
            }

            return satelliteOMM;
        }

        public async Task<Satellite?> GetOMMByName(string satelliteName)
        {
            LogLaunchByName(satelliteName);

            // Формируем уникальный ключ для кеша
            string cacheKey = $"omm:{satelliteName}";

            if (string.IsNullOrEmpty(satelliteName))
            {
                LogCancelNullByName();
                return null;
            }

            if (!_cache.TryGetValue(cacheKey, out Satellite? satelliteOMM))
            {
                // Если в кеше нет — один раз идем в базу данных
                satelliteOMM = await _dbContext.SatellitesTLE.AsNoTracking().FirstOrDefaultAsync(s => s.OBJECT_NAME == satelliteName);

                if (satelliteOMM != null)
                {
                    LogSuccessByName();

                    // Сохраняем в кеш 
                    var cacheOptions = new MemoryCacheEntryOptions()
                    // Спутник удалится из памяти через 30 минут
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
                    // Если этот спутник никто не смотрел 15 минут — выкидываем его, чтобы не забивать ОЗУ сервера
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15))
                    // Задаем высокий приоритет, чтобы сборщик мусора (GC) не снес его принудительно
                    .SetPriority(CacheItemPriority.High);

                    // Записываем OMM-сущность из базы в оперативную память сервера
                    _cache.Set(cacheKey, satelliteOMM, cacheOptions);
                }
                else
                {
                    // Сохраняем в короткий кеш для защиты от спама поиска несуществующих спутников
                    var cacheOptions = new MemoryCacheEntryOptions()
                    // Спутник удалится из памяти через 30 секунд
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

                    // Записываем OMM-сущность из базы в оперативную память сервера
                    _cache.Set(cacheKey, satelliteOMM, cacheOptions);

                    LogNotFoundByName(satelliteName);
                }
            }

            return satelliteOMM;
        }
    }
}
