using Core.Modules.SGP4Data.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SGP4Data.Domain.Interfaces
{
    public interface ISatelliteSGPRepository
    {
        Task<Satellite?> GetOMMByID(int noradId);

        Task<Satellite?> GetOMMByName(string satelliteName);
    }
}
