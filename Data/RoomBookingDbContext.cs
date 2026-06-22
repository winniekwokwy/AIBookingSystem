using Microsoft.EntityFrameworkCore;

public class RoomBookingDbContext : DbContext
{
    public RoomBookingDbContext(DbContextOptions<RoomBookingDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set;}
    public DbSet<AccessLog> AccessLogs { get; set; }
    public DbSet<ChangeLog> ChangeLogs { get; set; }
}