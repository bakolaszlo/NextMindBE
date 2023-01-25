namespace NextMindBE.Model
{
    public class Ping
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Name { get; set; }
        public NextMindStatus Status { get; set; }
        public byte[] Position { get; set; }
        public string SessionId { get; set; }
    }
}
