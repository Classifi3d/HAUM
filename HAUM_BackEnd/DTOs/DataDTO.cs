namespace HAUM_BackEnd.Entities
{
    public class DataDTO
    {
        public DateTime Time { get; set; }
        public SensorTypeEnum Type { get; set; }
        public float DataValue { get; set; }
    }
}
