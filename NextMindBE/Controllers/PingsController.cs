using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NextMindBE.Data;
using NextMindBE.DTOs;
using NextMindBE.Model;

namespace NextMindBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PingsController : ControllerBase
    {
        private readonly ILogger<PingsController> _logger;
        private readonly ApplicationDbContext _context;
        private const float LOCKOUT_TRESHOLD = 20f;
        public PingsController(ApplicationDbContext context, ILogger<PingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> PostPing(List<SensorData> payload)
        {
            if(await CheckPayload(payload))
            {
                return Ok();
            }
            NotifyEvents.StartAlarm();
            return BadRequest();
        }

        private async Task<bool> CheckPayload(List<SensorData> payload)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(Request.Headers["Authorization"].ToString().Remove(0, 7)) as JwtSecurityToken;
            var claims = token?.Claims;

            var sessionId = claims?.FirstOrDefault(c => c.Type == "SessionId")?.Value;

            if (sessionId == null)
            {
                _logger.LogWarning($"Session id was not found. {payload}");
                return false;
            }

            var sessionHistory = _context.SessionHistory.FirstOrDefault(o => o.SessionId == sessionId);

            if (sessionHistory == null)
            {
                if(PingTimerManager._authenticatedUsers.TryGetValue(sessionId,out var _))
                {
                    PingTimerManager._authenticatedUsers.Remove(sessionId);
                }
                _logger.LogWarning($"No session history found. Expected session history with session id: {sessionId}");
                return false;
            }

            if (payload.Count == 0)
            {
                return true; // ?
            }

            if (!IsTimeDeltaValid(payload, sessionHistory,sessionId))
            {
                return false;
            }
            
            if (!IsSensorDataStillValid(payload[0].SensorValues, sessionId))
            {
                return false;
            }


            if (!PingTimerManager._authenticatedUsers.ContainsKey(sessionId))
            {
                _logger.LogError("Received an inactive ping.");
                return false;
            }


            PingTimerManager._authenticatedUsers[sessionHistory.SessionId].LastActive = DateTime.UtcNow;

            _context.SensorData.AddRange(payload);
            await _context.SaveChangesAsync();
            return true;
        }

        private bool IsTimeDeltaValid(List<SensorData> payload, SessionHistory sessionHistory, string sessionId)
        {
            var time1 = payload[0].RecordedTime;

            for (int i = 1; i < payload.Count; i++)
            {
                var deltaTime = time1 - payload[i].RecordedTime;
                if (Math.Abs(deltaTime.TotalSeconds) - sessionHistory.UpdateInterval > 0.09)
                {
                    _logger.LogWarning("Received a delayed timestamp. Cancelling.");
                    return false;
                }

                if (!IsSensorDataStillValid(payload[i].SensorValues, sessionId))
                {
                    return false;
                }

                time1 = payload[i].RecordedTime;
            }

            return true;
        }

        private bool IsSensorDataStillValid(float[] sensorValues, string sessionId)
        {
            var calibrationValues = _context.SensorOnCalibrationEnd.FirstOrDefault(o => o.SessionId == sessionId);
            if (calibrationValues == null)
            {
                _logger.LogError($"Could not find calibration values for session id: {sessionId}");
                return false;
            }

            for(int i = 0; i < sensorValues.Length; i++)
            {
                if (Math.Abs(sensorValues[i] - calibrationValues.SensorValues[i]) > LOCKOUT_TRESHOLD)
                {
                    _logger.LogError($"Sensor data is invalid at pos:{i}. Got: {sensorValues[i]} Calibration Value: {calibrationValues.SensorValues[i]} Threshold: {LOCKOUT_TRESHOLD}");
                    return false;
                }
            }
            return true;
        }
    }
}
