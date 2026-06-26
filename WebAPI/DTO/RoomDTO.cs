
namespace AIBookingSystem.DTO
{
    public class RoomDTO
    {
    public int Id {get; set;}
    public required string Name { get; set;}
    public required int Floor { get; set;}
    public required int Capacity { get; set;}
    public required string Description { get; set;}
   
    public ICollection<EquipmentDTO>? Equipments {get; set;} = [];
    //public ICollection<Booking>? Bookings { get; set; } = [];
    }
}