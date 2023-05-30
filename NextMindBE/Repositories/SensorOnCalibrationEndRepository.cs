using NextMindBE.Data;
using NextMindBE.Interfaces.Repository;
using NextMindBE.Model;

namespace NextMindBE.Repositories
{
    public class SensorOnCalibrationEndRepository : ISensorOnCalibrationEndRepository
    {
        private readonly ApplicationDbContext _context;

        public SensorOnCalibrationEndRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(SensorOnCalibrationEnd sensorOnCalibrationEnd)
        {
            _context.SensorOnCalibrationEnd.Add(sensorOnCalibrationEnd);
            _context.SaveChangesAsync();
        }

        public SensorOnCalibrationEnd? Get(string sessionId)
        {
            return _context.SensorOnCalibrationEnd.FirstOrDefault(x => x.SessionId == sessionId);
        }
    }
}
