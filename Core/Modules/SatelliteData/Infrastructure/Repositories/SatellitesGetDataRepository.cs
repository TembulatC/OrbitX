using Core.Modules.SatelliteData.Domain.Interfaces;
using Core.Modules.SatelliteData.Domain.Models;
using Core.Modules.SatelliteData.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Modules.SatelliteData.Infrastructure.Repositories
{
    public class SatellitesGetDataRepository : ISatellitesGetDataRepository
    {
        private readonly OMMDBContext _dbContext;
        private readonly IMemoryCache _cache;
        
        public SatellitesGetDataRepository(OMMDBContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<List<Satellite>> GetSatellitesFiltersById(string category, int page, int pageSize = 25)
        {
            if (page < 1) page = 1;

            if (string.IsNullOrEmpty(category)) return new List<Satellite>();

            // Формируем уникальный ключ для кеша
            string cacheKey = $"satellites_list_{category.ToUpperInvariant()}_by_id";

            // Проверяем, есть ли уже данные в кеше
            if (!_cache.TryGetValue(cacheKey, out List<int>? satellitesIdsList))
            {
                satellitesIdsList = await _dbContext.Satellites
                    .Where(s => s.Category == category)
                    .OrderBy(s => s.NORAD_CAT_ID)
                    .Select(s => s.NORAD_CAT_ID)
                    .ToListAsync();

                if (satellitesIdsList != null || satellitesIdsList?.Count <= 0)
                {
                    // Сохраняем в кеш
                    var cacheOptions = new MemoryCacheEntryOptions()
                    // Категория удалится из памяти через 2 часа
                    .SetAbsoluteExpiration(TimeSpan.FromHours(2))
                    // Если эту категорию никто не смотрел 15 минут — выкидываем ее, чтобы не забивать ОЗУ сервера
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15))
                    // Задаем высокий приоритет, чтобы сборщик мусора (GC) не снес ее принудительно
                    .SetPriority(CacheItemPriority.High);

                    _cache.Set(cacheKey, satellitesIdsList, cacheOptions);
                }
                else return new List<Satellite>();
            }

            if (satellitesIdsList == null || !satellitesIdsList.Any()) return new List<Satellite>();

            // Делаем пагинацию прямо в памяти над списком ID
            int skipCount = (page - 1) * pageSize;
            var pageIds = satellitesIdsList.Skip(skipCount).Take(pageSize).ToList();

            // Идем в базу ОДИН раз и по индексу забираем полные данные
            var satellitesList = await _dbContext.Satellites
                .Where(s => pageIds.Contains(s.NORAD_CAT_ID))
                .OrderBy(s => s.NORAD_CAT_ID)
                .ToListAsync();

            if (satellitesList == null || !satellitesList.Any()) return new List<Satellite>();

            return satellitesList;
        }

        public async Task<List<Satellite>> GetSatellitesFiltersByName(string category, int page, int pageSize = 25)
        {
            if (page < 1) page = 1;

            if (string.IsNullOrEmpty(category)) return new List<Satellite>();

            // Формируем уникальный ключ для кеша
            string cacheKey = $"satellites_list_{category.ToUpperInvariant()}_by_name";

            // Проверяем, есть ли уже данные в кеше
            if (!_cache.TryGetValue(cacheKey, out List<int>? satellitesIdsList))
            {
                satellitesIdsList = await _dbContext.Satellites
                    .Where(s => s.Category == category)
                    .OrderBy(s => s.OBJECT_NAME)
                    .Select(s => s.NORAD_CAT_ID)
                    .ToListAsync();

                if (satellitesIdsList != null || satellitesIdsList?.Count <= 0)
                {
                    // Сохраняем в кеш 
                    var cacheOptions = new MemoryCacheEntryOptions()
                    // Категория удалится из памяти через 2 часа
                    .SetAbsoluteExpiration(TimeSpan.FromHours(2))
                    // Если эту категорию никто не смотрел 15 минут — выкидываем ее, чтобы не забивать ОЗУ сервера
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15))
                    // Задаем высокий приоритет, чтобы сборщик мусора (GC) не снес ее принудительно
                    .SetPriority(CacheItemPriority.High);

                    _cache.Set(cacheKey, satellitesIdsList, cacheOptions);
                }
                else return new List<Satellite>();
            }

            if (satellitesIdsList == null || !satellitesIdsList.Any()) return new List<Satellite>();

            // Делаем пагинацию прямо в памяти над списком ID
            int skipCount = (page - 1) * pageSize;
            var pageIds = satellitesIdsList.Skip(skipCount).Take(pageSize).ToList();

            // Идем в базу ОДИН раз и по индексу забираем полные данные
            var satellitesList = await _dbContext.Satellites
                .Where(s => pageIds.Contains(s.NORAD_CAT_ID))
                .OrderBy(s => s.OBJECT_NAME)
                .ToListAsync();

            if (satellitesList == null || !satellitesList.Any()) return new List<Satellite>();
            
            return satellitesList;
        }
    }
}
