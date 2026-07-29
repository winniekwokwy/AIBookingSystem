using AIBookingSystem.DTO;
using AIBookingSystem.Services;
using Microsoft.AspNetCore.Mvc;
namespace AIBookingSystem.Controllers
{
    [ApiController] 
    [Route("api/[controller]")] 
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        // Constructor receives IUserService via Dependency Injection
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(UserLoginDTO loginDto)
        {
            // Validate input model (email, password, clientId)
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Return 400 with validation errors
            // Get client IP address for logging and refresh token generation
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            // Call UserService to authenticate user and get JWT + refresh tokens
            var authResponse = await _userService.AuthenticateUser(loginDto, ipAddress);
            // If authentication fails (invalid credentials or client), return 401 Unauthorized
            if (authResponse == null)
                return Unauthorized("Invalid credentials or client.");
            // Successful login: return 200 OK with tokens and expiry info
            return Ok(authResponse);
        }
        // POST api/auth/refresh-token
        // Endpoint to obtain a new access token using a refresh token
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDTO>> RefreshToken(RefreshTokenRequestDTO refreshRequest)
        {
            // Validate input model (refreshToken and clientId required)
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Return 400 with validation errors
            // Get client IP address (optional for logging/auditing)
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            // Call UserService to validate refresh token and issue new access & refresh tokens
            var authResponse = await _userService.RefreshToken(refreshRequest.RefreshToken, refreshRequest.ClientId, ipAddress);
            // If refresh token or client is invalid, return 401 Unauthorized
            if (authResponse == null)
                return Unauthorized("Invalid refresh token or client." );
            // Successful token refresh: return 200 OK with new tokens and expiry info
            return Ok(authResponse);
        }
    }
}