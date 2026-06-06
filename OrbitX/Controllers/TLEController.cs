using Core.Modules.TLEData.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace OrbitX.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TLEController : ControllerBase
    {
        private readonly ITLEDataService _tLEDataService;

        public TLEController(ITLEDataService tLEDataService)
        {
            _tLEDataService = tLEDataService;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task AddTLEData(string satellitesCategory)
        {
            await _tLEDataService.AddTLEData(satellitesCategory);
        }
    }
}
