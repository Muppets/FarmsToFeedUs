using FarmsToFeedUs.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmsToFeedUs.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FarmController : ControllerBase
    {
        [HttpGet]
        public Task<List<Farm>> Get()
        {
            //var importData = await FarmDataService.GetFarmData();

            //var postcode = await PostcodeService.GetPostcodeAsync("GU34 1JT");

            //var meters = latLong.GetDistanceTo(new GeoCoordinate(51.150010, -0.974426));

            return Task.FromResult(new List<Farm>());
        }
    }
}
