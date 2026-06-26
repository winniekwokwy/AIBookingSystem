using System.ComponentModel.DataAnnotations.Schema;

public class Equipment
{
    public int Id {get; set;}
    public required string Name { get; set;}

    [ForeignKey ("Id")]
    public required int RoomId {get; set;}
    public Room? Room {get; set;}
}