using Core.Modules.SatelliteData.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SatelliteData.Domain.Interfaces
{
    public interface ISatellitesDataRepository
    {
        Task AddSatelliteData(List<Satellite> tle, string satellitesCategory);
        Task AddSatelliteData(List<Satellite> tle, string satellitesCategory, CancellationToken cancellationToken);

    }
}
