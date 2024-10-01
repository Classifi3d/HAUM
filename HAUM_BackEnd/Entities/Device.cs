namespace HAUM_BackEnd.Entities
{
    public class Device
    {
        // Properties 
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        // Table Connections
        public User User { get; set; } = null!;
        public Guid UserId { get; set; }
        public List<Data> Datas { get; set; } = new List<Data>();
    }
}
