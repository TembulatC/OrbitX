using Core.Modules.SGP4Data.Domain.Interfaces;
using Core.Modules.SGP4Data.Domain.Models;
using Core.Modules.SGP4Data.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
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

        public SatelliteSGPRepository(SGP4DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SatelliteTLE?> GetTLEByID(int noradId)
        {
            return await _dbContext.SatellitesTLE.FindAsync(noradId);
        }

        public async Task<SatelliteTLE?> GetTLEByName(string satelliteName)
        {
            var satelliteTLE = await _dbContext.SatellitesTLE.AsNoTracking().FirstOrDefaultAsync(s => s.Name == satelliteName);
            return satelliteTLE;
        }
    }
}
