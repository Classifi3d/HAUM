using Microsoft.Extensions.Hosting;

namespace HAUM_BackEnd.Entities
{
    public class User
    {
        // Properties 
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        // Table Connections
        public ICollection<Device>? Devices { get; set; } = new List<Device>();
    }
} 
