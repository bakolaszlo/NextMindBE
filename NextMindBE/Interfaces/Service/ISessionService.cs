using Microsoft.VisualBasic;
using NextMindBE.Model;
using System;

namespace NextMindBE.Interfaces.Service
{
    public interface ISessionService
    {
        void Create(Guid guid, int userId, double updateInterval);
        bool Check(string sessionId);
        SessionHistory? Get(string sessionId);

    }
}
