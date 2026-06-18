using AIBookingSystem.Enums;
using Microsoft.EntityFrameworkCore;

[Index(nameof(UserName), IsUnique = true)]
public class User
{
    public int Id { get; set;}
    public required string Name {get; set;}
    public required string UserName { get; set;}
    public required string Password { get; set;}
    public required UserRoles Role { get; set;}
    public required UserStatus Status { get; set;}
    public ICollection<Booking>? Bookings { get; set;}

}