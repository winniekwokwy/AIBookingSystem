using AIBookingSystem.Enums;
using AIBookingSystem.Models;

using Microsoft.EntityFrameworkCore;

[Index(nameof(UserName), IsUnique = true)]
public class User
{
    public int Id { get; set;}
    public required string Name {get; set;}
    public required string UserName { get; set;}
    public required string PasswordHash { get; set;}
    public required UserRoles Role { get; set;}
    public required UserStatus Status { get; set;}
    public ICollection<Booking> Bookings { get; set;} = [];
     public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

}