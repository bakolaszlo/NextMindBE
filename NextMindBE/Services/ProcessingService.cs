using Newtonsoft.Json;
using NextMindBE.Controllers;
using NextMindBE.Interfaces.Service;
using NextMindBE.Model;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace NextMindBE.Services
{
    public class ProcessingService : IProcessingService
    {
        private readonly ICipher _cipherService;
        private readonly ISessionService _sessionService;
        private readonly IValidatorService _validatorService;
        private readonly ILogger<ProcessingService> _logger;

        public ProcessingService(ICipher cipherService, ISessionService sessionService, IValidatorService validatorService, ILogger<ProcessingService> logger)
        {
            _cipherService = cipherService;
            _sessionService = sessionService;
            _validatorService = validatorService;
            _logger = logger;
        }

        public bool ProcessPayload(HttpRequest request, byte[] payload, PayloadType payloadType)
        {
            switch (payloadType)
            {
                case PayloadType.Pulse:
                    
                    return HandlePulseData(request, payload);
                case PayloadType.Sensor:
                    var decipheredPayloadByte = _cipherService.Cipher(payload, DHController.sharedKey);
                    return HandleSensorData(request, decipheredPayloadByte);
                default:
                    break;
            }

            return false;
        }

        private bool HandlePulseData(HttpRequest request, byte[] decipheredPayloadByte)
        {
            var sensorData = JsonConvert.DeserializeObject<List<float>>(Encoding.UTF8.GetString(decipheredPayloadByte));
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(request.Headers["Authorization"].ToString().Remove(0, 7)) as JwtSecurityToken;
            var claims = token?.Claims;
            var sessionId = claims?.FirstOrDefault(c => c.Type == "SessionId")?.Value;


            if (!_sessionService.Check(sessionId))
            {
                _logger.LogError("Session is not valid.");
                return false;
            }

            if (!PingTimerManager._authenticatedUsers.ContainsKey(sessionId))
            {
                _logger.LogError("Received an inactive ping.");
                return false;
            }

            if (!_validatorService.ValidatePulseData(sensorData, sessionId))
            {
                _logger.LogError("Data validation failed.");
                return false;
            }
            return true;
        }

        bool HandleSensorData(HttpRequest request, byte[] decipheredPayloadByte)
        {
            var sensorData = JsonConvert.DeserializeObject<List<SensorData>>(Encoding.UTF8.GetString(decipheredPayloadByte));
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(request.Headers["Authorization"].ToString().Remove(0, 7)) as JwtSecurityToken;
            var claims = token?.Claims;
            var sessionId = claims?.FirstOrDefault(c => c.Type == "SessionId")?.Value;


            if (!_sessionService.Check(sessionId))
            {
                _logger.LogError("Session is not valid.");
                return false;
            }

            if (!PingTimerManager._authenticatedUsers.ContainsKey(sessionId))
            {
                _logger.LogError("Received an inactive ping.");
                return false;
            }

            if (!_validatorService.ValidateSensorData(sensorData, sessionId))
            {
                _logger.LogError("Data validation failed.");
                return false;
            }
            return true;


        }
    }
}
