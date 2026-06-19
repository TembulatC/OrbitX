using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.TLEData.Domain.Interfaces.Services
{
    public interface ISatellitesService
    {
        Task AddTLEData(string satellitesCategory);
    }
}
