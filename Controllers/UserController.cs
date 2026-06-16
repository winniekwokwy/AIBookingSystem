using Microsoft.AspNetCore.Mvc;

namespace AIBookingSystem.Controllers;

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
        var userDTOs = _dBContext.Users.ToList();
        if (userDTOs == null)
        {
            return NotFound(new { Message = "No user is found." });
        }
        return Ok(userDTOs);
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDTO>> GetUser([FromQuery] UserDTO userDTO)
    {
        if (userDTO != null)
        {
            if (userDTO.Id != 0) 
            {

                var user = _dBContext.Users.FirstOrDefault(u => u.Id == userDTO.Id);
                if (user == null)
                {
                    return NotFound(new { Message = $"User with ID {userDTO.Id} not found." });

                }
                return Ok(new UserDTO
                {
                    Id = user.Id,
                    Name = user.UserName,
                    UserName = user.UserName,
                    Role = user.Role,
                    Status = user.Status
                });
            }
            else if (userDTO.UserName != null)
            {
                var user = _dBContext.Users.FirstOrDefault(u => u.UserName == userDTO.UserName);
                if (user == null)
                {
                    return NotFound(new { Message = $"User with UserName {userDTO.UserName} not found." });

                }
                return Ok(new UserDTO
                {
                    Id = user.Id,
                    Name = user.UserName,
                    UserName = user.UserName,
                    Role = user.Role,
                    Status = user.Status
                });
            }
        }
        return NotFound(new { Message = "Please provide valid Id or user name for getting user." });
    }

    [HttpPost]
    public ActionResult<UserDTO> CreateUser([FromBody] UserCreateDTO createDto)
    {
        string message="";

        if (UserService.IsUserValid(createDto.UserId, _dBContext))
        {
            if (!UserService.IsRoleValid(createDto.Role))
            {
                message = "Role can be User or Admin only";
            }
            else if (UserService.UsernameExsited(createDto.UserName, _dBContext))
            {
                message = "The user name is used. Please choose anothe one.";
            }
            else
            {
                var newUser = new User
                                {
                                    Name = createDto.Name,
                                    UserName = createDto.UserName,
                                    Role = createDto.Role,
                                    Password = createDto.Password,
                                    Status = "Active"
                                };

                var addUserResult = _dBContext.Users.Add(newUser);

                var newLog = new ChangeLog
                                {
                                    EntityType = "User",
                                    UserId = createDto.UserId,
                                    Action = "Add"
                                };
                var addLogResult = _dBContext.ChangeLogs.Add(newLog);
                
                _dBContext.SaveChanges();

                return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
            }
        }
        else
        {
            message = "The user doesn't have right to create a user.";
        }
        return BadRequest(new { Error = message });
    }
}