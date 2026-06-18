using System.ComponentModel.DataAnnotations;
using AIBookingSystem.Enums;

namespace AIBookingSystem.DTO
{
    public class UserCreateDTO
    {
        [Required(ErrorMessage = "Name of the user is required.")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Name of the user must be between 10 and 50 characters.")]
        public required string Name {get; set;}
        
        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username can only be composed of aplanumeric. No special character is allowed.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Username must be between 8 and 20 characters.")]
        public required string UserName { get; set;}
    
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[^\\s<>]{8,}$", ErrorMessage = "Password must be composed of at least one small letter, capital letter, number and special characters.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters")]  
        public required string Password { get; set;}
        
        [Required(ErrorMessage = "Role is required")]
        [AllowedValues("User", "Admin", ErrorMessage = "Role can only be User or Admin.")]
        public required string Role { get; set;}
        public required string CreatedBy {get; set;}
        public required int UserId {get; set;}
        
        [AllowedValues("Active", ErrorMessage = "Status can only be Active.")]
        public string Status {get; set;} = "Active";
    }
}