using Microsoft.EntityFrameworkCore;

public class RoomBookingDbContext : DbContext
{
    public RoomBookingDbContext(DbContextOptions<RoomBookingDbContext> options) : base(options)
    {
        
    }

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<User>()
    //         .Property(o => o.Status)
    //         .HasConversion<string>();

    //     modelBuilder.Entity<Booking>()
    //         .Property(o => o.Status)
    //         .HasConversion<string>();
    // }

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