namespace NextMindBE.Exceptions
{
    public class SessionHistoryNotFoundException : Exception
    {
        public SessionHistoryNotFoundException()
        {
        }

        public SessionHistoryNotFoundException(string message)
            : base(message)
        {
        }

        public SessionHistoryNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
