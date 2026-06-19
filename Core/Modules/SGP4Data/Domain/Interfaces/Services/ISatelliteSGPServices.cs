using Core.Modules.SGP4Data.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SGP4Data.Domain.Interfaces.Services
{
    public interface ISatelliteSGPServices
    {
        Task<SatelliteTLE?> GetSGPByID(int noradId);

        Task<SatelliteTLE?> GetSGPByName(string satelliteName);
    }
}
