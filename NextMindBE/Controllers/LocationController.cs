using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg.Sig;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NextMindBE.Controllers
{
    public class LocationData
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        public static LocationData locationData = new LocationData();
        [HttpPost]
        public IActionResult UpdateLocation([FromBody] LocationData location)
        {
            LocationController.locationData.latitude = location.latitude;
            LocationController.locationData.longitude = location.longitude;
            return Ok();
        }
    }
}
