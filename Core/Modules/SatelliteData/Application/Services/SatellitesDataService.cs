using Core.Modules.SatelliteData.Application.Interfaces;
using Core.Modules.SatelliteData.Domain.Interfaces;
using Core.Modules.SatelliteData.Domain.Models;
using Core.Modules.SatelliteData.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace Core.Modules.SatelliteData.Application.Services
{
    public partial class SatellitesDataService : ISatellitesService
    {
        private readonly ISatellitesDataRepository _SatelliteData;
        private readonly HttpSatellitesData _httpData;
        private readonly SatellitesParserService _satellitesParserService;
        private readonly ILogger<SatellitesDataService> _logger;

        public SatellitesDataService(ISatellitesDataRepository SatelliteData, HttpSatellitesData httpSatelliteData, SatellitesParserService satellitesParserService, ILogger<SatellitesDataService> logger)
        {
            _SatelliteData = SatelliteData;
            _httpData = httpSatelliteData;
            _satellitesParserService = satellitesParserService;
            _logger = logger;
        }

        public async Task AddSatelliteData(string satellitesCategory)
        {
            LogLaunch();

            string httpTLEstring = await _httpData.GetSatelliteData(satellitesCategory);

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

            await _SatelliteData.AddSatelliteData(tle, satellitesCategory);

            LogEnding();
        }
    }
}
