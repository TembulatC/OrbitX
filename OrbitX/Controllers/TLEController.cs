using Core.Modules.SGP4Data.Application.Interfaces;
using Core.Modules.TLEData.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OrbitX.BackgroundWorkers;

namespace OrbitX.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public partial class TLEController : ControllerBase
    {
        private readonly ISatellitesService _tLEDataService;
        private readonly ISatelliteSGPServices _satelliteSGPServices;
        private readonly SatelliteBackgroundWorker _worker;
        private readonly ILogger<TLEController> _logger;

        public TLEController(ISatellitesService tLEDataService, ISatelliteSGPServices satelliteSGPServices, SatelliteBackgroundWorker worker, ILogger<TLEController> logger)
        {
            _tLEDataService = tLEDataService;
            _satelliteSGPServices = satelliteSGPServices;
            _worker = worker;
            _logger = logger;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddTLEData(string satellitesCategory)
        {
            LogLaunchAdd(satellitesCategory);

            if (string.IsNullOrWhiteSpace(satellitesCategory))
            {
                LogCancelOperation();
                return BadRequest("Название категории спутников не может быть пустым");
            }

            await _tLEDataService.AddTLEData(satellitesCategory);

            LogSuccessAdd();
            return Ok();
        }

        [HttpGet("position-by-id/{noradId:int}")]
        public async Task<IActionResult> GetSGP4DataById(int noradId)
        {
            LogLaunchGetById(noradId);

            if (noradId < 0)
            {
                LogCancelNegativeNumber();
                return BadRequest("NoradId не может быть отрицательным");
            }

            var satelliteSPG = await _satelliteSGPServices.GetSGPByID(noradId);

            if (satelliteSPG == null)
            {
                LogCancelNullById();
                return NotFound($"Данных о спутнике с ID {noradId} не существует либо произошел сбой в математических расчетах SGP4");
            }

            LogSuccessGetById();
            return Ok(satelliteSPG);
        }

        [HttpGet("position-by-name/{satelliteName}")]
        public async Task<IActionResult> GetSGP4DataByName(string satelliteName)
        {
            LogLaunchGetByName(satelliteName);

            if (string.IsNullOrWhiteSpace(satelliteName))
            {
                LogCancelOperationName();
                return BadRequest("Имя спутника не может быть пустым");
            }

            var satelliteSPG = await _satelliteSGPServices.GetSGPByName(satelliteName.ToUpper());

            if (satelliteSPG == null)
            {
                LogCancelNullByName();
                return NotFound($"Данных о спутнике {satelliteName} не существует либо произошел сбой в математических расчетах SGP4");
            }

            LogSuccessGetByName();
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
