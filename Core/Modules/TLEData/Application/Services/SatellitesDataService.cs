using Core.Modules.TLEData.Application.Interfaces;
using Core.Modules.TLEData.Domain.Interfaces;
using Core.Modules.TLEData.Domain.Models;
using Core.Modules.TLEData.Infrastructure.HttpClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.TLEData.Application.Services
{
    public class SatellitesDataService : ISatellitesService
    {
        private readonly ISatellitesDataRepository _tleData;
        private readonly HttpSatellitesData _httpData;

        public SatellitesDataService(ISatellitesDataRepository tleData, HttpSatellitesData httpTLEData)
        {
            _tleData = tleData;
            _httpData = httpTLEData;
        }

        public async Task AddTLEData(string satellitesCategory)
        {
            string httpTLEstring = await _httpData.GetTLEData(satellitesCategory);
            List<Satellite> tle = SatellitesParserService.Parse(httpTLEstring, satellitesCategory);

            await _tleData.AddTLEData(tle, satellitesCategory);
        }
    }
}
