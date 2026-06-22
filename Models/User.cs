using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

public class User
{
    public int Id { get; set;}
    public required string Name {get; set;}
    public required string UserName { get; set;}
    public required string Password { get; set;}
    public required string Role { get; set;}
    public required string Status { get; set;} = "Active";
    public ICollection<Booking> Bookings { get; set;} = null!;
    public ICollection<ChangeLog> Changes { get; set;} = null!;
    public ICollection<AccessLog> Access { get; set; } = null!;
}