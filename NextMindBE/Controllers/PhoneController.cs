using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NextMindBE.Data;
using NextMindBE.Interfaces.Service;
using NextMindBE.Model;
using NextMindBE.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NextMindBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PhoneController : ControllerBase
    {
        private readonly ILogger<PingsController> _logger;
        private readonly IProcessingService _processingService;

        public PhoneController(IProcessingService processingService, ILogger<PingsController> logger)
        {
            _processingService = processingService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> PostPing([FromBody] List<float> payload)
        {
            if (CheckPayload(payload))
            {
                return Ok();
            }
            NotifyEvents.StartAlarm();
            return BadRequest();
        }

        private bool CheckPayload(List<float> payload)
        {
            string jsonString = JsonConvert.SerializeObject(payload);
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            return _processingService.ProcessPayload(Request, byteArray, PayloadType.Pulse);
        }
    }
}
