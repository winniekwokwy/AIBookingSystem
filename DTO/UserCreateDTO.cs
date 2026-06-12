using System.ComponentModel.DataAnnotations;

public class UserCreateDTO
{
    [Required(ErrorMessage = "Name of the user is required.")]
    [StringLength(50, MinimumLength = 10, ErrorMessage = "Name of the user must be between 10 and 50 characters.")]
    public required string Name {get; set;}
    
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Username must be between 8 and 20 characters.")]
    public required string UserName { get; set;}
   
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password should be at least 6 characters")]  
    public required string Password { get; set;}
    
    [Required(ErrorMessage = "Role is required")]
    public required string Role { get; set;}
    public string Status {get; set; } = "Active";
}