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
            _logger.LogInformation($"Запуск цикла обновления данных спутников категории - {satellitesCategory}");

            if (string.IsNullOrWhiteSpace(satellitesCategory))
            {
                _logger.LogWarning("Отмена операции. Передано пустое или некорректное имя категории спутников");
                return BadRequest("Название категории спутников не может быть пустым");
            }

            await _tLEDataService.AddTLEData(satellitesCategory);

            _logger.LogInformation("Результат цикла: Успех");
            return Ok("Успех");
        }

        [HttpGet("position-by-id/{noradId:int}")]
        public async Task<IActionResult> GetSGP4DataById(int noradId)
        {
            _logger.LogInformation($"Запуск цикла получения данных о спутнике {noradId} на текущий момент");

            if (noradId < 0)
            {
                _logger.LogWarning("Отмена операции. Передан отрицательный NoradId");
                return BadRequest("NoradId не может быть отрицательным");
            }

            var satelliteSPG = await _satelliteSGPServices.GetSGPByID(noradId);

            if (satelliteSPG == null)
            {
                return NotFound($"Данных о спутнике с ID {noradId} не существует либо произошел сбой в математических расчетах SGP4");
            }
            
            _logger.LogInformation("Результат цикла: Успех");
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
