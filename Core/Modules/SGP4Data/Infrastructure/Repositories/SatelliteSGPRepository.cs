using Core.Modules.SGP4Data.Domain.Interfaces;
using Core.Modules.SGP4Data.Domain.Models;
using Core.Modules.SGP4Data.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SGP4Data.Infrastructure.Repositories
{
    public class SatelliteSGPRepository : ISatelliteSGPRepository
    {
        private readonly SGP4DBContext _dbContext;
        private readonly ILogger<SatelliteSGPRepository> _logger;

        public SatelliteSGPRepository(SGP4DBContext dbContext, ILogger<SatelliteSGPRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<SatelliteTLE?> GetTLEByID(int noradId)
        {
            _logger.LogInformation("Запуск получения данных спутника из базы данных");

            var satelliteTLE = await _dbContext.SatellitesTLE.FindAsync(noradId);

            if (satelliteTLE != null) _logger.LogInformation("Данные о спутнике успешно получены");
            else _logger.LogWarning($"Спутника с ID {noradId} не существует в базе данных");

            return satelliteTLE;
        }

        public async Task<SatelliteTLE?> GetTLEByName(string satelliteName)
        {
            var satelliteTLE = await _dbContext.SatellitesTLE.AsNoTracking().FirstOrDefaultAsync(s => s.Name == satelliteName);
            return satelliteTLE;
        }
    }
}
