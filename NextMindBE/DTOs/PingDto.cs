namespace NextMindBE.DTOs
{
    public class PingDto
    {
        public NextMindStatus Status { get; set; }
        public byte[] Position { get; set; }

        public string SessionId { get; set; }
    }
}
