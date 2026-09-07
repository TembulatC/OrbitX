using Core.Modules.TLEData.Application.Services;
using Core.Modules.TLEData.Domain.Interfaces;
using Core.Modules.TLEData.Domain.Models;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Logging;
using SGPdotNET.TLE;

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

            int requestCount = 1;

            foreach (string category in CelestrakCategories)
            {
                string url = (category == "gpz" || category == "gpz-plus")
                ? $"https://celestrak.org/NORAD/elements/gp.php?SPECIAL={category}&FORMAT=cvs"
                : $"https://celestrak.org/NORAD/elements/gp.php?GROUP={category}&FORMAT=cvs";

                int seconds = Random.Shared.Next(30, 61);

                LogHttpRequest(requestCount, seconds, url);

                await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);

                try
                {
                    await ProcessCategoryResponse(client, url, category, cancellationToken);
                    requestCount++;
                }
                catch (HttpRequestException ex)
                {
                    LogUnknownHttpError(ex);
                    requestCount++;
                    continue;
                }
                catch (Exception ex)
                {
                    LogOtherError(ex);
                    requestCount++;
                    continue;
                }
            }
        }

        private async Task ProcessCategoryResponse(HttpClient client, string url, string category, CancellationToken cancellationToken)
        {
            using HttpResponseMessage response = await client.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                string omm = await response.Content.ReadAsStringAsync(cancellationToken);

                if (string.IsNullOrEmpty(omm))
                {
                    LogHTTPBodyNull();
                    return;
                }
                else if (omm.Contains("invalid", StringComparison.OrdinalIgnoreCase) || omm.Contains("error", StringComparison.OrdinalIgnoreCase))
                {
                    LogInvalid();
                    return;
                }
;
                await SaveDataToRepository(omm, category, cancellationToken);

                LogSuccess();
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

        // Отдельный метод для обращения к БД
        private async Task SaveDataToRepository(string omm, string category, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(omm))
            {
                LogHTTPBodyNull();
                return;
            }

            List<Satellite> parseData = _satellitesParserService.Parse(omm, category);

            if (parseData == null || parseData.Count <= 0)
            {
                LogParseBodyNull();
                return;
            }

            await _satellitesDataRepository.AddTLEData(parseData, category, cancellationToken);
        }
    }
}
