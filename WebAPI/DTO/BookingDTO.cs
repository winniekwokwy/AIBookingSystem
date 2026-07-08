using AIBookingSystem.Enums;

namespace AIBookingSystem.DTO
{
    public class BookingDTO
    {
        public int Id {get; set;}
        public required string BookedBy { get; set;}
        public required int UserId { get; set;}
        public required int RoomId { get; set;}
        public required DateTimeOffset BookingFrom {get; set;}
        public required DateTimeOffset BookingTo {get; set;}
        public string? Status {get; set;}
    }
}