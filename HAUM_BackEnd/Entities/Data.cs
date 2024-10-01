namespace HAUM_BackEnd.Entities
{
    public class Data
    {
        // Properties 
        public DateTime Time { get; set; }
        public SensorTypeEnum Type { get; set; }
        public float DataValue { get; set; }
        // Table Connections
        public Device Device { get; set; } = null!;
        public Guid DeviceId { get; set; }

    }
}
