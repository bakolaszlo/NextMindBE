using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextMindBE.Data;
using System.IdentityModel.Tokens.Jwt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NextMindBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TriggerController : ControllerBase
    {
        private readonly ILogger<TriggerController> _logger;

        public TriggerController(ILogger<TriggerController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void Post()
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(Request.Headers["Authorization"].ToString().Remove(0, 7)) as JwtSecurityToken;
            var claims = token?.Claims;

            var sessionId = claims?.FirstOrDefault(c => c.Type == "SessionId")?.Value;

            if (sessionId == null)
            {
                NotifyEvents.Trigger(State.Deny);
                return;
            }

            if (!PingTimerManager._authenticatedUsers.TryGetValue(sessionId, out var _))
            {
                _logger.LogError($"Received invalid request from session id: {sessionId}");
                NotifyEvents.Trigger(State.Deny);
                return;
            }

            NotifyEvents.Trigger(State.Authorize);
        }

    }
}
