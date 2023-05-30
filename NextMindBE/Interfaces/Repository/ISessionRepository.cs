using NextMindBE.Model;

namespace NextMindBE.Interfaces.Repostory
{
    public interface ISessionRepository
    {
        void Add(SessionHistory session);
        SessionHistory Get(string sessionId);
    }
}
