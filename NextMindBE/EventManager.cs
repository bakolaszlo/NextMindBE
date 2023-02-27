using NextMindBE.Controllers;
using NextMindBE.Data;
using NuGet.Protocol;

namespace NextMindBE
{
    public enum State
    {
        Authorize,
        Deny,
        Idle
    }

    public class NotifyEvents
    {
        
        public static void StartAlarm()
        {
            Dictionary<string, bool> data = new Dictionary<string, bool>()
            {
                { "Alarm", true }
            };

            TriggerData.Data = data;
        }

        public static void Trigger(State state)
        {
            Dictionary<string, string> data = new Dictionary<string, string> {
                {
                    "Alarm", Enum.GetName(typeof(State), state)!
                },
                { "Latitude", LocationController.locationData.latitude.ToString() },
                { "Longitude", LocationController.locationData.longitude.ToString() }
            };
            TriggerData.Data = data;
        }
    }


}
