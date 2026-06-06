using Core.Modules.TLEData.Domain.Interfaces.Repositories;
using Core.Modules.TLEData.Domain.Interfaces.Services;
using Core.Modules.TLEData.Domain.Models;
using Core.Modules.TLEData.Infrastructure.HttpClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.TLEData.Application.Services
{
    public class TLEDataService : ITLEDataService
    {
        private readonly ITLEData _tleData;
        private readonly HttpTLEData _httpData;

        public TLEDataService(ITLEData tleData, HttpTLEData httpTLEData)
        {
            _tleData = tleData;
            _httpData = httpTLEData;
        }

        public async Task AddTLEData(string satellitesCategory)
        {
            string httpTLEstring = await _httpData.GetTLEData(satellitesCategory);

            List<Satellite> tle = TLEParser.Parse(httpTLEstring, satellitesCategory);

            await _tleData.AddTLEData(tle, satellitesCategory);
        }
    }
}
