namespace NextMindBE.Model
{
    public class SessionHistory
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime Created { get; set; }

        public double UpdateInterval { get; set; }
    }
}
