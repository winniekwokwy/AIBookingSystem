using Microsoft.AspNetCore.Mvc;
using AIBookingSystem.DTO;
using AIBookingSystem.Services;

namespace AIBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogService _logService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger, ILogService logService)
    {
        _userService = userService;
        _logger = logger;
        _logService = logService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDTO>> ListUsers()
    {
        var users = _userService.ListUsers();
        if (users == null)
        {
            return NotFound(new { Message = "No user is found." });
        }

        return Ok(users);
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDTO>> GetUserByID(int id)
    {
        if (id > 0) 
        {
            var user = _userService.GetUserbyID(id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with ID, {id}, not found." });

            }
            return Ok(user);
        }

        return NotFound(new { Message = "Please provide valid Id for getting user." });
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDTO>> GetUserByUsername(string userName)
    {
        if (userName != null)
        {
            var user = _userService.GetUserbyUsername(userName);
            if (user == null)
            {
                return NotFound(new { Message = $"User with User name, {userName}, is not found." });

            }
            return Ok(user);
        }
        return NotFound(new { Message = "Please provide valid user name for getting user." });
    }

    [HttpPost]
    public ActionResult<UserDTO> CreateUser([FromBody] UserCreateDTO createDto)
    {
         string message="";

        if (_userService.IsUserValid(createDto))
        {
            if (!_userService.IsRoleValid(createDto.Role))
            {
                message = "Role can be User or Admin only";
            }
            else if (_userService.UsernameExsited(createDto.UserName))
            {
                message = "The user name is in use. Please choose anothe one.";
            }
            else
            {
                var newUser = _userService.CreateUser(createDto);
                if (newUser == null)
                {
                    message = "User is not created successfully.";
                }
                else {
                    var addLogSuccess = _logService.AddUserChangeLog(createDto);
                    if (addLogSuccess)
                    {
                        return CreatedAtAction(nameof(GetUserByID), new { id = newUser.Id }, newUser);   
                    }
                    else
                    {
                        message = "User is not created successfully as change log is failed to update.";
                    }
                    
                }
            }
        }
        else
        {
            message = "The user is not a valid user or User Id and Username does not match. Please check.";
        }
        return BadRequest(new { Error = message });
    }
}