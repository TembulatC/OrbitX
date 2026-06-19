using Core.Modules.SGP4Data.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SGP4Data.Domain.Interfaces.Repositories
{
    public interface ISatelliteSGPRepository
    {
        Task<SatelliteTLE?> GetTLEByID(int noradId);

        Task<SatelliteTLE?> GetTLEByName(string satelliteName);
    }
}
