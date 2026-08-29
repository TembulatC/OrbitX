using Core.Modules.TLEData.Application.Services;
using Core.Modules.TLEData.Domain.Interfaces;
using Core.Modules.TLEData.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Text;

namespace OrbitX.BackgroundWorkers.Helper
{
    public class SatelliteTLEDownloader
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISatellitesDataRepository _satellitesDataRepository;
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

        public SatelliteTLEDownloader(IHttpClientFactory httpClientFactory, ISatellitesDataRepository satellitesDataRepository, SatellitesParserService satellitesParserService)
        {
            _httpClientFactory = httpClientFactory;
            _satellitesDataRepository = satellitesDataRepository;
            _satellitesParserService = satellitesParserService;
        }

        public async Task GetTLEData(CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient();
            Random random = new Random();

            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36");

            foreach (string category in CelestrakCategories)
            {               
                int seconds = random.Next(5, 31);
                string url;

                await Task.Delay(seconds * 1000, cancellationToken);

                if (category == "gpz" || category == "gpz-plus") url = $"https://celestrak.org/NORAD/elements/gp.php?SPECIAL={category}&FORMAT=tle";
                else url = $"https://celestrak.org/NORAD/elements/gp.php?GROUP={category}&FORMAT=tle";
                    
                string tle = await client.GetStringAsync(url, cancellationToken);
                
                await _satellitesDataRepository.AddTLEData(_satellitesParserService.Parse(tle, category), category, cancellationToken);
            }
        }
    }
}
