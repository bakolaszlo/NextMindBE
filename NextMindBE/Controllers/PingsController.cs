using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NextMindBE.Data;
using NextMindBE.DTOs;
using NextMindBE.Interfaces.Service;
using NextMindBE.Model;

namespace NextMindBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PingsController : ControllerBase
    {
        private readonly ILogger<PingsController> _logger;
        private readonly IProcessingService _processingService;
        public PingsController(ILogger<PingsController> logger, IProcessingService processingService)
        {
            _logger = logger;
            _processingService = processingService;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> PostPing([FromBody] string payload)
        {
            if(CheckPayload(payload))
            {
                return Ok();
            }
            NotifyEvents.StartAlarm();
            return BadRequest();
        }

        private bool CheckPayload(string OTPMessage)
        {
            return _processingService.ProcessPayload(Request, Convert.FromBase64String(OTPMessage), PayloadType.Sensor);
        }
             
    }
}
