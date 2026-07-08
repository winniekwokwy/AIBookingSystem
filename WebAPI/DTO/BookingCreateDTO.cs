using System.ComponentModel.DataAnnotations;

namespace AIBookingSystem.DTO
{
    public class BookingCreateDTO
    {
        [Required(ErrorMessage = "BookedBy is required.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username can only be composed of aplanumeric. No special character is allowed.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Username must be between 8 and 20 characters.")]
        public required string BookedBy { get; set;}

        [Required(ErrorMessage = "User Id is required.")]
        public required int UserId { get; set;}

        [Required(ErrorMessage = "Room Id is required.")]
        public required int RoomId { get; set;}

        [Required(ErrorMessage = "Start date and time of the booking is required.")]
        public required DateTimeOffset BookingFrom {get; set;}
        
        [Required(ErrorMessage = "End date and time of the booking is required.")]
        public required DateTimeOffset BookingTo {get; set;}
    }
}