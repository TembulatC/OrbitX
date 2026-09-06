using Core.Modules.TLEData.Application.Services;
using Core.Modules.TLEData.Domain.Interfaces;
using Core.Modules.TLEData.Domain.Models;
using Microsoft.Extensions.Logging;

namespace OrbitX.BackgroundWorkers.Helper
{
    public partial class SatelliteTLEDownloader
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISatellitesDataRepository _satellitesDataRepository;
        private readonly ILogger<SatelliteTLEDownloader> _logger;
        private readonly SatellitesParserService _satellitesParserService;
        private readonly static string[] CelestrakCategories = new[]
        {
            "weather", "resource", "sar", "sarsat", "dmc", "tdrss", "argos",
            "planet", "spire", "geo", "gpz", "gpz-plus", "intelsat", "ses",
            "eutelsat", "telesat", "starlink", "oneweb", "qianfan", "hulianwang",
            "kuiper", "iridium-NEXT", "orbcomm", "globalstar", "amateur",
            "satnogs", "x-comm", "other-comm", "gnss", "gps-ops", "glo-ops",
            "galileo", "beidou", "sbas", "science", "geodetic", "engineering",
            "education", "military", "radar", "cubesat"
        };

        public SatelliteTLEDownloader(IHttpClientFactory httpClientFactory, ISatellitesDataRepository satellitesDataRepository, ILogger<SatelliteTLEDownloader> logger, SatellitesParserService satellitesParserService)
        {
            _httpClientFactory = httpClientFactory;
            _satellitesDataRepository = satellitesDataRepository;
            _logger = logger;
            _satellitesParserService = satellitesParserService;
        }

        public async Task GetTLEData(CancellationToken cancellationToken)
        {
            using HttpClient client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36");

            foreach (string category in CelestrakCategories)
            {               
                int seconds = Random.Shared.Next(15, 35);
                string url = (category == "gpz" || category == "gpz-plus")
                ? $"https://celestrak.org/NORAD/elements/gp.php?SPECIAL={category}&FORMAT=tle"
                : $"https://celestrak.org/NORAD/elements/gp.php?GROUP={category}&FORMAT=tle";

                await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);

                try
                {
                    await ProcessCategoryResponse(client, url, category, cancellationToken);
                }
                catch (HttpRequestException ex)
                {

                }
            }
        }

        private async Task ProcessCategoryResponse(HttpClient client, string url, string category, CancellationToken cancellationToken)
        {
            using HttpResponseMessage response = await client.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                string tle = await response.Content.ReadAsStringAsync(cancellationToken);
                await SaveDataToRepository(tle, category, cancellationToken);
                return;
            }

            int statusCode = (int)response.StatusCode;

            switch (statusCode)
            {
                case 403:
                    Log403StatusCode(statusCode);
                    break;
                case 404:
                    Log404StatusCode(statusCode);
                    break;
                case 500:
                    Log500StatusCode(statusCode);
                    break;
                case 503:
                    Log503StatusCode(statusCode);
                    break;
                default:
                    LogUnknownError(statusCode, response.ReasonPhrase);
                    break;
            }
        }

        private async Task SaveDataToRepository(string tle, string category, CancellationToken cancellationToken)
        {
            List<Satellite> parseData = _satellitesParserService.Parse(tle, category);
            await _satellitesDataRepository.AddTLEData(parseData, category, cancellationToken);
        }
    }
}
