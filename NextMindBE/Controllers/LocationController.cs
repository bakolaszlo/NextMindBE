using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg.Sig;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NextMindBE.Controllers
{
    public class LocationData
    {
        public float latitude;
        public float longitude;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        public static LocationData locationData = new LocationData();
        [HttpPost]
        public IActionResult UpdateLocation([FromBody] LocationData locationData)
        {
            LocationController.locationData = locationData;
            return Ok();
        }
    }
}
