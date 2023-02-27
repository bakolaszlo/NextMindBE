using NuGet.Protocol;

namespace NextMindBE.Data
{
    internal class TriggerData
    {
        private static object? _data;
        internal static object Data
        {
            get { return _data.ToJson(); }
            set { _data = value; }
        }
    }
}