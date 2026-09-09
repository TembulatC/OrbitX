using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.TLEData.Application.Interfaces
{
    public interface ISatellitesService
    {
        Task AddTLEData(string satellitesCategory);
    }
}
