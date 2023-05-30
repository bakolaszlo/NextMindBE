using Microsoft.EntityFrameworkCore;
using NextMindBE.Data;
using NextMindBE.Exceptions;
using NextMindBE.Interfaces.Repostory;
using NextMindBE.Model;

namespace NextMindBE.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ApplicationDbContext _context;

        public SessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(SessionHistory session)
        {
            _context.SessionHistory.Add(session);
            _context.SaveChanges();
        }

        public SessionHistory Get(string sessionId)
        {
            var sessionHistory = _context.SessionHistory.FirstOrDefault(o => o.SessionId == sessionId);
            if (sessionHistory != null)
            {
                return sessionHistory;
            }
            throw new SessionHistoryNotFoundException($"Session not found: {sessionId}");
        }
    }
}
