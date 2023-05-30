using NextMindBE.Interfaces.Service;
using NextMindBE.Model;

namespace NextMindBE.Services
{
    public class ValidatorService : IValidatorService
    {
        private readonly IValidator<SensorData> _sensorDataValidator;
        private readonly IValidator<float> _pulseDataValidator;

        public ValidatorService(IValidator<SensorData> sensorDataValidator, IValidator<float> pulseDataValidator)
        {
            _sensorDataValidator = sensorDataValidator;
            _pulseDataValidator = pulseDataValidator;
        }

        public bool ValidateSensorData(List<SensorData> data, string sessionId)
        {
            return _sensorDataValidator.ValidateData(data, sessionId);
        }

        public bool ValidatePulseData(List<float> data, string sessionId)
        {
            return _pulseDataValidator.ValidateData(data, sessionId);
        }
    }

}
