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
        private readonly ApplicationDbContext _context;
        private const float LOCKOUT_TRESHOLD = 20f;
        public PingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> PostPing(List<SensorData> payload)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(Request.Headers["Authorization"].ToString().Remove(0, 7)) as JwtSecurityToken;
            var claims = token?.Claims;

            var sessionId = claims.FirstOrDefault(c => c.Type == "SessionId").Value;

            var sessionHistory = _context.SessionHistory.FirstOrDefault(o => o.SessionId == sessionId);
            if(sessionHistory == null)
            {
                Console.WriteLine($"No session Id found. Expected: {sessionId}");
                return BadRequest(); 
                // TODO: Remove from the list, and lock out the app.
            }

            if(payload.Count == 0)
            {
                return Ok();
            }

            var time1 = payload[0].RecordedTime;
            if(!IsSensorDataStillValid(payload[0].SensorValues, sessionId))
            {
                return BadRequest();
            }

            for (int i = 1; i < payload.Count; i++)
            {
                var deltaTime = time1 - payload[i].RecordedTime;
                if(Math.Abs(deltaTime.TotalSeconds) - sessionHistory.UpdateInterval > 0.09)
                {
                    Console.WriteLine("Received a delayed timestamp.");
                    return BadRequest();
                }

                if (!IsSensorDataStillValid(payload[i].SensorValues, sessionId))
                {
                    return BadRequest();
                }

                time1 = payload[i].RecordedTime;
            }

            if (!PingTimerManager._authenticatedUsers.ContainsKey(sessionId))
            {
                // TODO: Notify lockout server to lock out the app.
                Console.WriteLine("Recived an inactive ping. Alarm ON!");
                return BadRequest();
            }


            PingTimerManager._authenticatedUsers[sessionHistory.SessionId].LastActive = DateTime.UtcNow;

            _context.SensorData.AddRange(payload);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool IsSensorDataStillValid(float[] sensorValues, string sessionId)
        {
            var calibrationValues = _context.SensorOnCalibrationEnd.FirstOrDefault(o => o.SessionId == sessionId);
            if (calibrationValues == null)
            {
                Console.WriteLine($"Could not find calibration values for session id: {sessionId}");
                return false;
            }

            for(int i = 0; i < sensorValues.Length; i++)
            {
                if (Math.Abs(sensorValues[i] - calibrationValues.SensorValues[i]) > LOCKOUT_TRESHOLD)
                {
                    Console.WriteLine($"Sensor data is invalid at pos:{i}. Got: {sensorValues[i]} Calibration Value: {calibrationValues.SensorValues[i]} Threshold: {LOCKOUT_TRESHOLD}");
                    return false;
                }
            }
            return true;
        }
    }
}
