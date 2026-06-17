using System.ComponentModel.DataAnnotations.Schema;

public class Room
{
    public int Id {get; set;}
    public required string Name { get; set;}
    public required int Floor { get; set;}
    public required int Capacity { get; set;}
    public required string Description { get; set;}
    public string[] Equipment { get; set;} = [];
    public ICollection<ChangeLog> Changes { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = null!;
}