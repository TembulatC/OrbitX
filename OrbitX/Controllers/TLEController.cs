using Core.Modules.SGP4Data.Application.Interfaces;
using Core.Modules.SGP4Data.Domain.Interfaces.Services;
using Core.Modules.TLEData.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace OrbitX.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class TLEController : ControllerBase
    {
        private readonly ISatellitesService _tLEDataService;
        private readonly ISGP _sgp;

        public TLEController(ISatellitesService tLEDataService, ISGP sgp)
        {
            _tLEDataService = tLEDataService;
            _sgp = sgp;
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
            var satelliteSPG = await _sgp.GetSGP(noradId);
            return Ok(satelliteSPG);
        }

        [HttpGet("position-by-name/{satelliteName}")]
        public async Task<IActionResult> GetSGP4DataByName(string satelliteName)
        {
            var satelliteSPG = await _sgp.GetSGP(satelliteName.ToUpper());
            return Ok(satelliteSPG);
        }
    }
}
