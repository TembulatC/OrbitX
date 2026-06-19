using Core.Modules.TLEData.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.TLEData.Domain.Interfaces.Repositories
{
    public interface ISatellitesDataRepository
    {
        Task AddTLEData(List<Satellite> tle, string satellitesCategory);
    }
}
