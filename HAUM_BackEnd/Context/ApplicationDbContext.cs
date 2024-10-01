using HAUM_BackEnd.Entities;
using Microsoft.EntityFrameworkCore;


namespace HAUM_BackEnd.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<User>().HasIndex(x => x.Id).IsUnique();
            modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
            modelBuilder.Entity<Device>().ToTable("Device");
            modelBuilder.Entity<Device>().HasKey(x => x.Id);
            modelBuilder.Entity<Device>().HasIndex(x => x.Id).IsUnique();
            modelBuilder.Entity<Data>().ToTable("Data");
            modelBuilder.Entity<Data>().HasKey(x => new { x.Time, x.Type } );

            modelBuilder.Entity<Device>()
                .HasOne(d => d.User)
                .WithMany(u => u.Devices)
                .HasForeignKey(d => d.UserId);

            modelBuilder.Entity<Data>()
                .HasOne(s => s.Device)
                .WithMany(d => d.Datas)
                .HasForeignKey(s => s.DeviceId);

        }

        public DbSet<User> User { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<Data> Data { get; set; }
    }
}
