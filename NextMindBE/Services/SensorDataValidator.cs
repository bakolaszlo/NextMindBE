using Microsoft.EntityFrameworkCore;
using NextMindBE.Controllers;
using NextMindBE.Data;
using NextMindBE.Interfaces.Repository;
using NextMindBE.Interfaces.Repostory;
using NextMindBE.Interfaces.Service;
using NextMindBE.Model;

namespace NextMindBE.Services
{
    public class SensorDataValidator : IValidator<SensorData>
    {
        private readonly ILogger<SensorDataValidator> _logger;
        private readonly float LOCKOUT_TRESHOLD = 30f;
        private readonly ISensorOnCalibrationEndRepository _sensorOnCalibrationEnd;
        private readonly ISessionRepository _session;
        private readonly ISensorService _sensorService;


        public SensorDataValidator(ILogger<SensorDataValidator> logger, ISensorOnCalibrationEndRepository sensorOnCalibrationEnd, ISessionRepository session, ISensorService sensorService)
        {
            _logger = logger;
            _sensorOnCalibrationEnd = sensorOnCalibrationEnd;
            _session = session;
            _sensorService = sensorService;
        }


        public bool ValidateData(List<SensorData> data, string sessionId)
        {
            return IsTimeDeltaValid(data, sessionId);
        }

        private bool IsTimeDeltaValid(List<SensorData> payload, string sessionId)
        {
            if(payload.Count == 0)
            {
                return true;
            }

            var sessionHistory = _session.Get(sessionId);
            if (sessionHistory == null)
            {
                if (PingTimerManager._authenticatedUsers.TryGetValue(sessionId, out var _))
                {
                    PingTimerManager._authenticatedUsers.Remove(sessionId);
                }
                _logger.LogWarning($"No session history found. Expected session history with session id: {sessionId}");
                return false;
            }

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

            if (!IsSensorDataStillValid(payload[0].SensorValues, sessionId))
            {
                return false;
            }

            PingTimerManager._authenticatedUsers[sessionHistory.SessionId].LastActive = DateTime.UtcNow;

            _sensorService.Add(payload);
            return true;
        }

        private bool IsSensorDataStillValid(float[] sensorValues, string sessionId)
        {
            var calibrationValues = _sensorOnCalibrationEnd.Get(sessionId);
            if (calibrationValues == null)
            {
                _logger.LogError($"Could not find calibration values for session id: {sessionId}");
                return false;
            }

            for (int i = 0; i < sensorValues.Length; i++)
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
