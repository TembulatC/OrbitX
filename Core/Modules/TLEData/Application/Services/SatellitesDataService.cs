using Core.Modules.TLEData.Application.Interfaces;
using Core.Modules.TLEData.Domain.Interfaces;
using Core.Modules.TLEData.Domain.Models;
using Core.Modules.TLEData.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;
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
            _logger.LogInformation("Цикл запущен");

            string httpTLEstring = await _httpData.GetTLEData(satellitesCategory);

            if (string.IsNullOrEmpty(httpTLEstring))
            {
                _logger.LogWarning("Отмена запуска парсера. Строка данных пришла пустой");
                return;
            }

            List<Satellite> tle = _satellitesParserService.Parse(httpTLEstring, satellitesCategory);

            if (tle.Count == 0)
            {
                _logger.LogWarning("Отмена запуска операции добавления данных в базу данных. Список спутников вернулся пустым");
                return;
            }

            await _tleData.AddTLEData(tle, satellitesCategory);

            _logger.LogInformation("Цикл окончен");
        }
    }
}
