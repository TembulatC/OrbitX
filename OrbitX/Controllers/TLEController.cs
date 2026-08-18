using Core.Modules.SGP4Data.Application.Interfaces;
using Core.Modules.TLEData.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OrbitX.BackgroundWorkers;

namespace OrbitX.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class TLEController : ControllerBase
    {
        private readonly ISatellitesService _tLEDataService;
        private readonly ISatelliteSGPServices _satelliteSGPServices;
        private readonly SatelliteBackgroundWorker _worker;

        public TLEController(ISatellitesService tLEDataService, ISatelliteSGPServices satelliteSGPServices, SatelliteBackgroundWorker worker)
        {
            _tLEDataService = tLEDataService;
            _satelliteSGPServices = satelliteSGPServices;
            _worker = worker;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task AddTLEData(string satellitesCategory)
        {
            await _tLEDataService.AddTLEData(satellitesCategory);
        }

        [HttpGet("position-by-id/{noradId:int}")]
        public async Task<IActionResult> GetSGP4DataById(int noradId)
        {
            var satelliteSPG = await _satelliteSGPServices.GetSGPByID(noradId);
            return Ok(satelliteSPG);
        }

        [HttpGet("position-by-name/{satelliteName}")]
        public async Task<IActionResult> GetSGP4DataByName(string satelliteName)
        {
            var satelliteSPG = await _satelliteSGPServices.GetSGPByName(satelliteName.ToUpper());
            return Ok(satelliteSPG);
        }

        // Имитируем вход пользователя на страницу спутника
        [HttpPost("start-test/{noradId:int}")]
        public IActionResult StartWorkerThread(int noradId)
        {
            // Напрямую даем команду воркеру запустить параллельный поток расчета
            _worker.OnSatelliteWatched(noradId);

            return Ok($"Сигнал старта отправлен для ID: {noradId}");
        }

        // Имитируем выход пользователя со страницы
        [HttpPost("stop-test/{noradId:int}")]
        public IActionResult StopWorkerThread(int noradId)
        {
            // Даем команду воркеру затушить параллельный поток
            _worker.OnSatelliteUnwatched(noradId);

            return Ok($"Сигнал остановки отправлен для ID: {noradId}");
        }
    }
}
