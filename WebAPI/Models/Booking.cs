using System.ComponentModel.DataAnnotations.Schema;
using AIBookingSystem.Enums;

namespace AIBookingSystem.Models
{
    public class Booking
    {
        public int Id {get; set;}

        public required string BookedBy { get; set;}

        [ForeignKey("Id")]
        public required int UserId { get; set;}
        public required User User { get; set;}

        [ForeignKey("Id")]
        public required int RoomId { get; set;}
        public required Room Room { get; set;}
        public required DateTime BookingTime {get; set;}
        public required BookingStatus Status {get; set;}
    }
}