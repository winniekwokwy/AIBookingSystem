using Microsoft.AspNetCore.Mvc;
using AIBookingSystem.DTO;
using AIBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;

namespace AIBookingSystem.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
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
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<UserDTO>> ListUsers()
        {
            var users = _userService.ListUsers();
            if (users == null || users.Count() == 0)
            {
                string message = "No user is found.";
                _logger.LogError(message);
                return NotFound(message);
            }

            return Ok(users);
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDTO>> GetUserbyID(int id)
        {
            string message;
            if (id > 0) 
            {
                var user = _userService.GetUserbyID(id);
                if (user == null)
                {
                    message = $"User with ID, {id}, not found.";
                    _logger.LogError(message);
                    return NotFound(message);

                }
                return Ok(user);
            }

            message = "Please provide valid Id for getting user.";
            _logger.LogError(message);
            return NotFound(message);
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDTO>> GetUserbyUsername(string userName)
        {
            string message;
            if (userName != null)
            {
                var user = _userService.GetUserbyUsername(userName.ToLower());
                if (user == null)
                {
                    
                    message = $"User with User name, {userName}, is not found.";
                    _logger.LogError(message);
                    return NotFound(message);

                }
                return Ok(user);
            }
            message = "Please provide valid user name for getting user.";
            _logger.LogError(message);        
            return NotFound(message);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<UserDTO> CreateUser([FromBody] UserCreateDTO createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); 

            string message="";

            if (createDto.Role == "")
            {
                message = "Please provide user role.";
            }
            else
            {
                if (!_userService.IsRoleValid(createDto.Role))
                {
                    message = "Role can be User or Admin only.";
                }
                else 
                {
                    if (createDto.Status == ""){
                        message = "Please provide user status.";
                    }
                    else 
                    {
                        if (!_userService.IsStatusValid(_userService.StatusMappingString2Enum(createDto.Status)))
                        {
                            message = "Status can be Active or Inactive only.";
                        }
                        else
                        {
                            var newUser = _userService.CreateUser(createDto);
                            if (newUser == null)
                            {
                                message = "User is not created successfully. Username is in use. Please choose another one.";
                            }
                            else {

                                return CreatedAtAction(nameof(GetUserbyID), new { id = newUser.Id }, newUser);   
                                
                            }
                        }
                    }
                }
            }
            _logger.LogError(message);        
            return BadRequest (message);
        }
    }
}