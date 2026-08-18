using Core.Modules.TLEData.Domain.Interfaces;
using Core.Modules.TLEData.Domain.Models;
using Core.Modules.TLEData.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.TLEData.Infrastructure.Repositories
{
    public class SatellitesDataRepository : ISatellitesDataRepository
    {
        private readonly TLEDBContext _dbContext;

        public SatellitesDataRepository(TLEDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddTLEData(List<Satellite> tle, string satellitesCategory)
        {
            // Поиск по категории одним запросом
            // База найдет их по индексу за долю миллисекунды и выдаст пачкой
            var existingSatellites = await _dbContext.Satellites
                .Where(s => s.Category == satellitesCategory)
                .ToDictionaryAsync(s => s.NoradId); // Переводим в Dictionary для быстрого поиска

            foreach (Satellite satelliteTle in tle)
            {
                // Ищем в памяти по ID
                if (existingSatellites.TryGetValue(satelliteTle.NoradId, out var existing))
                {
                    // Спутник найден в этой категории — обновляем свойства
                    existing.Name = satelliteTle.Name;
                    existing.TLELine1 = satelliteTle.TLELine1;
                    existing.TLELine2 = satelliteTle.TLELine2;
                    existing.Epoch = satelliteTle.Epoch;
                    existing.UpdatedAt = satelliteTle.UpdatedAt;
                }
                else
                {
                    // Новая запись — готовим к добавлению
                    satelliteTle.Category = satellitesCategory;
                    _dbContext.Satellites.Add(satelliteTle);
                }
            }

            // Сохраняем всё одним мощным батчем
            await _dbContext.SaveChangesAsync();
        }
    }
}
