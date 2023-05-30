using NextMindBE.Model;

namespace NextMindBE.Interfaces.Repository
{
    public interface ISensorOnCalibrationEndRepository
    {
        SensorOnCalibrationEnd? Get(string sessionId);
        void Add(SensorOnCalibrationEnd sensorOnCalibrationEnd);
    }
}
