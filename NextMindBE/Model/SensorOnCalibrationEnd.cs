﻿namespace NextMindBE.Model
{
    public class SensorOnCalibrationEnd
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public float[] SensorValues { get; set; }
    }
}
