using NextMindBE.Data;
using NextMindBE.Interfaces.Service;
using NextMindBE.Model;

namespace NextMindBE.Services
{
    public class SensorService : ISensorService
    {
        private readonly ApplicationDbContext _context;

        public SensorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(List<SensorData> data)
        {
            _context.SensorData.AddRange(data);
            _context.SaveChanges();
        }
    }
}
