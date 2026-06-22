using Microsoft.EntityFrameworkCore;

using AIBookingSystem.Models;

namespace AIBookingSystem.Data
{
    public class RoomBookingDbContext : DbContext
    {
        public RoomBookingDbContext(DbContextOptions<RoomBookingDbContext> options) : base(options)
        {
            
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
    
            configurationBuilder.Properties<Enum>().HaveConversion<string>();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set;}
        public DbSet<Equipment> Equipments {get; set;}
    }
}