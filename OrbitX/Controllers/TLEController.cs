using Core.Modules.SGP4Data.Application.Interfaces;
using Core.Modules.SatelliteData.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OrbitX.BackgroundWorkers;
using Serilog.Context;

namespace OrbitX.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public partial class TLEController : ControllerBase
    {
        private readonly ISatellitesService _satelliteDataService;
        private readonly ISatellitesGetService _satelliteGetService;
        private readonly ISatelliteSGPServices _satelliteSGPServices;
        private readonly SatelliteBackgroundWorker _worker;
        private readonly ILogger<TLEController> _logger;

        public TLEController(ISatellitesService satelliteDataService, ISatellitesGetService satelliteGetService, ISatelliteSGPServices satelliteSGPServices, SatelliteBackgroundWorker worker, ILogger<TLEController> logger)
        {
            _satelliteDataService = satelliteDataService;
            _satelliteGetService = satelliteGetService;
            _satelliteSGPServices = satelliteSGPServices;
            _worker = worker;
            _logger = logger;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddSatelliteData(string satellitesCategory)
        {
            LogLaunchAdd(satellitesCategory);

            if (string.IsNullOrWhiteSpace(satellitesCategory))
            {
                LogCancelOperation();
                return BadRequest("Название категории спутников не может быть пустым");
            }

            await _satelliteDataService.AddSatelliteData(satellitesCategory);

            LogSuccessAdd();
            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
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

        [HttpGet]
        [Route("[action]")]
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

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetSatellitesFiltersById(string category, int page, int pageSize = 25)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return BadRequest("Категория спутников не может быть пустой");
            }

            var satellitesList = await _satelliteGetService.GetSatellitesFiltersById(category, page, pageSize);

            if (satellitesList == null || satellitesList.Count <= 0)
            {
                return NotFound($"Данных о категории {category} не существует");
            }

            return Ok(satellitesList);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetSatellitesFiltersByName(string category, int page, int pageSize = 25)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return BadRequest("Категория спутников не может быть пустой");
            }

            var satellitesList = await _satelliteGetService.GetSatellitesFiltersByName(category, page, pageSize);

            if (satellitesList == null || satellitesList.Count <= 0)
            {
                return NotFound($"Данных о категории {category} не существует");
            }

            return Ok(satellitesList);
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
