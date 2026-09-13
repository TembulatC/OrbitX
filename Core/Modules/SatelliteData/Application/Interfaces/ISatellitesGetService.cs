using Core.Modules.SatelliteData.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SatelliteData.Application.Interfaces
{
    public interface ISatellitesGetService
    {
        Task<List<SatellitesFilterDTO>> GetSatellitesFiltersById(string category, int page, int pageSize = 25);
        Task<List<SatellitesFilterDTO>> GetSatellitesFiltersByName(string category, int page, int pageSize = 25);
    }
}
