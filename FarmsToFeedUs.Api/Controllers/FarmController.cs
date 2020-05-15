using FarmsToFeedUs.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmsToFeedUs.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FarmController : ControllerBase
    {
        private FarmImportService GoogleService { get; }

        private ILogger<FarmController> Logger { get; }

        public FarmController(FarmImportService googleService, ILogger<FarmController> logger)
        {
            GoogleService = googleService;
            Logger = logger;
        }

        [HttpGet]
        public async Task<List<FarmData>> Get()
        {
            return await GoogleService.GetFarmData();
        }
    }
}
