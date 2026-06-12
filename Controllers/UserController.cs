using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIBookingSystem.Controllers;

//  public required string Name {get; set;}
//     public required string UserName { get; set;}
//     public required string Password { get; set;}
//     public required string Role { get; set;}
//     public required string Status { get; set;} = "Active";
//     public ICollection<Booking> Bookings { get; set;} = null!;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly RoomBookingDbContext _dBContext;
    private readonly ILogger<UserController> _logger;

    public UserController(RoomBookingDbContext dBContext, ILogger<UserController> logger)
    {
        _dBContext = dBContext;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDTO>> ListUsers()
    {
        var userDTOs = _dBContext.Users.ToArrayAsync();
        return Ok(userDTOs);
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDTO>> GetUser(UserDTO userDTO)
    {
        if (userDTO.Id != 0) {

            var user = _dBContext.Users.FirstOrDefault(u => u.Id == userDTO.Id);
            if (user != null)
            {
                return Ok(new UserDTO
                {
                    Id = user.Id,
                    Name = user.UserName,
                    UserName = user.UserName,
                    Role = user.Role,
                    Status = user.Status
                });
            }
            return NotFound(new { Message = $"User with ID {userDTO.Id} not found." });
        }
        else if (userDTO.UserName != null)
        {
            var user = _dBContext.Users.FirstOrDefault(u => u.UserName == userDTO.UserName);
            if (user != null)
            {
                return Ok(new UserDTO
                {
                    Id = user.Id,
                    Name = user.UserName,
                    UserName = user.UserName,
                    Role = user.Role,
                    Status = user.Status
                });
            }
            return NotFound(new { Message = $"User with ID {userDTO.Id} not found." });
        }
        return NotFound(new { Message = "Please provide Id or user name for getting user." });
    }
}
