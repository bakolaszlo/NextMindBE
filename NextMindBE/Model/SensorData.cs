namespace NextMindBE.Model
{
    public class SensorData
    {
        public int Id { get; set; }
        public float[] SensorValues { get; set; }
        public DateTime RecordedTime { get; set; }
    }
}
