using System.ComponentModel.DataAnnotations.Schema;

public class Booking
{
    public int Id;
    public required int UserId { get; set;}
    public required User User { get; set;}
    [ForeignKey("Id")]
    public required int RoomId { get; set;}
    public required Room Room { get; set;}
    [ForeignKey("Id")]
    public required DateTime BookingTime {get; set;}
    public required String Status {get; set;} = "Active";
    public ICollection<ChangeLog> Changes { get; set;} = null!;
}