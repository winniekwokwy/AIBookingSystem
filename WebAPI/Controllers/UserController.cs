using Microsoft.AspNetCore.Mvc;
using AIBookingSystem.DTO;
using AIBookingSystem.Services;

namespace AIBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDTO>> ListUsers()
    {
        var users = _userService.ListUsers();
        if (users == null)
        {
            string message = "No user is found.";
            _logger.LogError(message);
            return NotFound(new { Message = message });
        }

        return Ok(users);
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDTO>> GetUserByID(int id)
    {
        string message = "";
        if (id > 0) 
        {
            var user = _userService.GetUserbyID(id);
            if (user == null)
            {
                message = $"User with ID, {id}, not found.";
                _logger.LogError(message);
                return NotFound(new { Message = message });

            }
            return Ok(user);
        }

        message = "Please provide valid Id for getting user.";
        _logger.LogError(message);
        return NotFound(new { Message = message });
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDTO>> GetUserByUsername(string userName)
    {
        string message = "";
        if (userName != null)
        {
            var user = _userService.GetUserbyUsername(userName);
            if (user == null)
            {
                
                message = $"User with User name, {userName}, is not found.";
                _logger.LogError(message);
                return NotFound(new { Message = message });

            }
            return Ok(user);
        }
        message = "Please provide valid user name for getting user.";
        _logger.LogError(message);        
        return NotFound(new { Message = message });
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
            else if (createDto.Role == null)
            {
                message = "Status can be Active only.";
            }
            else if (!_userService.IsStatusValid(_userService.StatusMappingString2Enum(createDto.Role)))
            {
                message = "Status can be Active only.";
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

                    return CreatedAtAction(nameof(GetUserByID), new { id = newUser.Id }, newUser);   
                    
                }
            }
        }
        else
        {
            message = "The user is not a valid user or User Id and Username does not match. Please check.";
        }
        _logger.LogError(message);        
        return BadRequest(new { Error = message });
    }
}