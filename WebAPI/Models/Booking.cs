using System.ComponentModel.DataAnnotations.Schema;
using AIBookingSystem.Enums;
using NodaTime;

namespace AIBookingSystem.Models
{
    public class Booking
    {
        public int Id {get; set;}

        public required string BookedBy { get; set;}

        [ForeignKey("Id")]
        public required int UserId { get; set;}
        public User? User { get; set;}

        [ForeignKey("Id")]
        public required int RoomId { get; set;}
        public Room? Room { get; set;}
        public required DateTimeOffset BookingFrom {get; set;}
        public required DateTimeOffset BookingTo {get; set;}
        public required BookingStatus Status {get; set;}
    }
}