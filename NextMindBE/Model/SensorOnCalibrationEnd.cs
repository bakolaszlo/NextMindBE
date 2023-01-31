namespace NextMindBE.Model
{
    public class SensorOnCalibrationEnd
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public float[] SensorData { get; set; }
    }
}
