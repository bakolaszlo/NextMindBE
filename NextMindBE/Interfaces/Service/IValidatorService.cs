using NextMindBE.Model;

namespace NextMindBE.Interfaces.Service
{
    public interface IValidatorService
    {
        bool ValidateSensorData(List<SensorData> data, string sessionId);
        bool ValidatePulseData(List<float> data, string sessionId);
    }
}
