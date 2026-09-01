using Core.Modules.SGP4Data.Domain.Interfaces;
using Core.Modules.SGP4Data.Domain.Models;
using Core.Modules.SGP4Data.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Modules.SGP4Data.Infrastructure.Repositories
{
    public partial class SatelliteSGPRepository : ISatelliteSGPRepository
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
            LogLaunchById(noradId);

            if (noradId < 0)
            {
                LogNegativeNumber();
                return null;
            }

            var satelliteTLE = await _dbContext.SatellitesTLE.FindAsync(noradId);

            if (satelliteTLE != null) LogSuccessById();
            else LogNotFoundById(noradId);

            return satelliteTLE;
        }

        public async Task<SatelliteTLE?> GetTLEByName(string satelliteName)
        {
            LogLaunchByName(satelliteName);

            if (string.IsNullOrEmpty(satelliteName))
            {
                LogCancelNullByName();
                return null;
            }

            var satelliteTLE = await _dbContext.SatellitesTLE.AsNoTracking().FirstOrDefaultAsync(s => s.Name == satelliteName);

            if (satelliteTLE != null) LogSuccessByName();
            else LogNotFoundByName(satelliteName);

            return satelliteTLE;
        }
    }
}
