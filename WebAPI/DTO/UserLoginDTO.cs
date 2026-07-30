using System.ComponentModel.DataAnnotations;
namespace AIBookingSystem.DTO
{
    public class UserLoginDTO
    {
        // Email input from the user during login.
        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username can only be composed of aplanumeric. No special character is allowed.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Username must be between 8 and 20 characters.")]
        public string UserName { get; set; } = null!;
        // Password input from the user during login.
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[^\\s<>]{8,}$", ErrorMessage = "Password must be composed of at least one small letter, capital letter, number and special characters.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters")]  
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "ClientId is required.")]
        public string ClientId { get; set; } = null!;
    }
}