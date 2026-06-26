using System.ComponentModel.DataAnnotations;

namespace AIBookingSystem.DTO
{
    public class RoomCreateDTO
    {
        [Required(ErrorMessage = "Name of the room is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Name of the room must be between 5 and 20 characters.")]
        public required string Name {get; set;}

        [Required(ErrorMessage = "The location of the room is required.")]
        public required int Floor { get; set;}

        [Required(ErrorMessage = "Capacity of the room is required.")]
        public required int Capacity { get; set;}

        [Required(ErrorMessage = "Description of the room is required.")]
        [StringLength(400, MinimumLength = 10, ErrorMessage = "Decription of the room must be between 10 and 400 characters.")]
        public required string Description { get; set;}
   
        public ICollection<EquipmentDTO> Equipments {get; set;} = [];
    }
}