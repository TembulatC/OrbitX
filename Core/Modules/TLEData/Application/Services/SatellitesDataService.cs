using Core.Modules.TLEData.Application.Interfaces;
using Core.Modules.TLEData.Domain.Interfaces;
using Core.Modules.TLEData.Domain.Models;
using Core.Modules.TLEData.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace Core.Modules.TLEData.Application.Services
{
    public partial class SatellitesDataService : ISatellitesService
    {
        private readonly ISatellitesDataRepository _tleData;
        private readonly HttpSatellitesData _httpData;
        private readonly SatellitesParserService _satellitesParserService;
        private readonly ILogger<SatellitesDataService> _logger;

        public SatellitesDataService(ISatellitesDataRepository tleData, HttpSatellitesData httpTLEData, SatellitesParserService satellitesParserService, ILogger<SatellitesDataService> logger)
        {
            _tleData = tleData;
            _httpData = httpTLEData;
            _satellitesParserService = satellitesParserService;
            _logger = logger;
        }

        public async Task AddTLEData(string satellitesCategory)
        {
            LogLaunch();

            string httpTLEstring = await _httpData.GetTLEData(satellitesCategory);

            if (string.IsNullOrEmpty(httpTLEstring))
            {
                LogCancellationParser();
                return;
            }

            List<Satellite> tle = _satellitesParserService.Parse(httpTLEstring, satellitesCategory);

            if (tle.Count == 0)
            {
                LogCancellationDB();
                return;
            }

            await _tleData.AddTLEData(tle, satellitesCategory);

            LogEnding();
        }
    }
}
