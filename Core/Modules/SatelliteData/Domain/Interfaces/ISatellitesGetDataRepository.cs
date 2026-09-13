using Core.Modules.SatelliteData.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SatelliteData.Domain.Interfaces
{
    public interface ISatellitesGetDataRepository
    {
        Task<List<Satellite>> GetSatellitesFiltersById(string category, int page, int pageSize = 50);
        Task<List<Satellite>> GetSatellitesFiltersByName(string category, int page, int pageSize = 50);
    }
}
