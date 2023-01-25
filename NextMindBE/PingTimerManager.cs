using NextMindBE.Model;
using System.Timers;

namespace NextMindBE
{
    public class PingTimerManager
    {

        public static Dictionary<string, User> _authenticatedUsers = new Dictionary<string, User>();
        private static System.Timers.Timer aTimer;
        private static void SetTimer()
        {
            aTimer = new System.Timers.Timer(30_000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        public static void StartTimer()
        {
            SetTimer();
            aTimer.Start();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var inactiveUsers = _authenticatedUsers.Where(u => (DateTime.UtcNow - u.Value.LastActive).TotalSeconds > 20).ToList();

            foreach (var inactiveUser in inactiveUsers)
            {
                _authenticatedUsers.Remove(inactiveUser.Key);
            }

            Console.WriteLine($"Current number of active users: {_authenticatedUsers.Count}");
        }
    }
}
