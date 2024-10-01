namespace HAUM_BackEnd.Entities
{
    public class DeviceDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? IpAddress { get; set; }
        public int Port { get; set; }

    }
}
