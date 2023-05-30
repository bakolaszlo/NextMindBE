using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NextMindBE.Controllers;
using NextMindBE.Exceptions;
using NextMindBE.Interfaces.Repostory;
using NextMindBE.Interfaces.Service;
using NextMindBE.Model;
using System;
using System.Security;

namespace NextMindBE.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ILogger<SessionService> _logger;
        public SessionService(ISessionRepository sessionRepository, ILogger<SessionService> logger)
        {
            _sessionRepository = sessionRepository;
            _logger = logger;
        }

        public bool Check(string sessionId)
        {
            try
            {
                var sessionHistory = _sessionRepository.Get(sessionId);
                return true;
            }
            catch (SessionHistoryNotFoundException)
            {
                if (PingTimerManager._authenticatedUsers.TryGetValue(sessionId, out var _))
                {
                    PingTimerManager._authenticatedUsers.Remove(sessionId);
                }
                _logger.LogWarning($"No session history found. Expected session history with session id: {sessionId}");
                return false;
            }
        }

        public SessionHistory? Get(string sessionId)
        {
            if(Check(sessionId))
            {
                return _sessionRepository.Get(sessionId);
            }

            return null;
        }

        public void Create(Guid guid, int userId, double updateInterval)
        {
            var sessionHistory = new SessionHistory()
            {
                Created = DateTime.Now,
                SessionId = guid.ToString(),
                UserId = userId,
                UpdateInterval = updateInterval,
            };

            _sessionRepository.Add(sessionHistory);
        }
    }
}
