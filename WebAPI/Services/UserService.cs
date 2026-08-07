using System.Net;
using AIBookingSystem.DTO;
using AIBookingSystem.Enums;
using AIBookingSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AIBookingSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        private readonly ITokenService _tokenService;

        private readonly IClientCacheService _clientCacheService;

        private readonly IConfiguration _configuration;

        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepo, ITokenService tokenService, IClientCacheService clientCacheService, IConfiguration configuration, ILogger<UserService> logger)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
            _clientCacheService = clientCacheService;
            _configuration = configuration;
            _logger = logger;
        }

        public bool IsRoleValid(string role)
        {
            if ((role.ToLower() != "user") && role.ToLower() != "admin")
            {
                return false;
            }
            return true;
        }

        public bool IsStatusValid(UserStatus status)
        {
            if (Enum.IsDefined(typeof(UserStatus), status))
            {
                return true;
            }
            return false;
        }

        public string? StatusMappingEnum2String(UserStatus status)
        {
            if (IsStatusValid(status))
            {
                switch (status)
                {
                    case UserStatus.Active:
                        return "Active";
                    case UserStatus.Inactive:
                        return "Inactive";
                }
            }
            return null;
        }
        
        public UserStatus StatusMappingString2Enum(string status)
        {
            if (status.ToLower() == "active")
            {
                return UserStatus.Active;
            }
            else if (status.ToLower() == "inactive")
            {
                return UserStatus.Inactive;
            }
            return (UserStatus) (-1);
        }
        public bool IsRoleValid(UserRoles role)
        {
            if (Enum.IsDefined(typeof(UserRoles), role))
            {
                return true;
            }
            return false;
        }
        public string? RoleMappingEnum2String(UserRoles role)
        {
            if (IsRoleValid(role))
            {
                switch (role)
                {
                    case UserRoles.Admin:
                        return "Admin";
                    case UserRoles.User:
                        return "User";
                }
            }
            return null;
        }
        
        public UserRoles RoleMappingString2Enum(string role)
        {
            if (role.ToLower() == "admin")
            {
                return UserRoles.Admin;
            }
            else if (role.ToLower() == "user")
            {
                return UserRoles.User;
            }
            return (UserRoles) (-1);
        }

        public UserDTO? MapUser2DTO(User user)
        {
            if (user != null){
                return new UserDTO{
                    Id = user.Id,
                    Name = user.Name,
                    UserName = user.UserName,
                    Role = RoleMappingEnum2String(user.Role),
                    Status = StatusMappingEnum2String(user.Status)
                };
            }
            return null;
        }

        public IEnumerable<UserDTO>? ListUsers()
        {
            var users = _userRepo.ListUsers();

            if (users != null)
            {
                return (IEnumerable<UserDTO>?) users
                    .ToList()
                    .Select(u => MapUser2DTO(u));
            }
            return null;
        }
        
        public UserDTO? GetUserbyID(int id)
        {
            var user = _userRepo.GetUserbyID(id);
            if (user == null)
            {
                return null;
            }

            return MapUser2DTO(user);
        }

        public UserDTO? GetUserbyUsername(string userName)
        {

            var user = _userRepo.GetUserbyUsername(userName.ToLower());

            if (user == null)
            {
                return null;
            }
            
            return MapUser2DTO(user);
        }

        public UserDTO? CreateUser(UserCreateDTO user)
        {
            _logger.LogInformation("Debug | UserService | CreateUser | reached.");
            if (user == null){
                _logger.LogInformation("Debug | UserService | CreateUser | User is null.");
                return null;
            }

            if (user.UserName == null)
            {
                _logger.LogInformation("Debug | UserService | CreateUser | Username is null.");
                return null;
            }

            var username = user.UserName.ToLower();
            if (_userRepo.UsernameInUse(username))
            {
                _logger.LogInformation("Debug | UserService | CreateUser | Username is in use.");
                return null;
            }
            var newUser = new User
                            {
                                Name = user.Name,
                                UserName = username,
                                Role = RoleMappingString2Enum(user.Role),
                                PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                                Status = StatusMappingString2Enum(user.Status)
                            };
    
            var addedUser = _userRepo.CreateUser(newUser);

            if (addedUser == null)
            {
                _logger.LogInformation("Debug | UserService | CreateUser | User created unsuccessfully.");
                return null;
            }
            return MapUser2DTO(addedUser);
        }

        public bool IsUserValid(int id, string username)
        {
            if (id <= 0 || username == null || username == "")
            {
                return false;
            }

            var foundUserById = GetUserbyID(id);
            var foundUserByUsername = GetUserbyUsername(username.ToLower());
            if (foundUserById ==null || foundUserByUsername == null)
            {
                return false;
            }
            if (foundUserById.Id != foundUserByUsername.Id || foundUserById.UserName != foundUserByUsername.UserName)
            {
                return false;
            }
            return true;
        }

        public async Task<AuthResponseDTO?> AuthenticateUser(UserLoginDTO loginDto, string ipAddress)
        {
            if ((loginDto == null) || (ipAddress == null) || (ipAddress == ""))
                return null;

            if (!IPAddress.TryParse(ipAddress, out _))
                return null;

            // Retrieve user by email with roles eagerly loaded; only active users allowed
            var user = _userRepo.GetUserbyUsername(loginDto.UserName);
            // Verify user exists and password matches the stored hashed password
            // _logger.LogInformation($"[User Service | Authentiate user: user is {user?.UserName}]");
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
               return null; // Invalid credentials
            }

            // Extract the authenticated user's role name for inclusion in JWT claims
            var userRoleName = RoleMappingEnum2String(user.Role);
            List<string> roles = new List<string>();
            if (!string.IsNullOrEmpty(userRoleName)) roles.Add(userRoleName);
            // Retrieve client info by ClientId
            var client = await _clientCacheService.GetClientByClientId(loginDto.ClientId);
            if (client == null)
            {
                // Fail if client does not exist or is inactive
                return null;
            }
            // Generate JWT access token with user details, roles, and client info
            var accessToken = _tokenService.GenerateAccessToken(user, roles, out string jwtId, client);
            if (accessToken == null || accessToken =="")
                return null;
            // Generate refresh token linked to the generated JWT ID, client, user, and IP address
            var refreshToken = _tokenService.GenerateRefreshToken(ipAddress, jwtId, client, user.Id);
            if (refreshToken == null)
                return null;
            // Store the refresh token in the database for later validation and refresh workflows
            _tokenService.AddRefreshTokens(refreshToken);

            // Read access token expiration duration from config or fallback to 15 minutes
            var accessTokenExpiryMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;
            // Return the tokens and expiry info encapsulated in AuthResponseDTO
            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes)
            };
        }

        // Refreshes an expired access token using a valid refresh token and client ID
        public async Task<AuthResponseDTO?> RefreshToken(RefreshTokenRequestDTO refreshTokenRequestDTO, string ipAddress)
        {
            _logger.LogInformation("[User Service | Refresh Token: reached.]");
            if (refreshTokenRequestDTO.RefreshToken == null || refreshTokenRequestDTO.RefreshToken == "" || refreshTokenRequestDTO.ClientId == null || refreshTokenRequestDTO.ClientId == "" || ipAddress == null || ipAddress == "")
            {
                _logger.LogInformation("[User Service | Refresh Token: refreshToken or ClientId or ipAddress is null or empty.]");
                return null;
            }
            // Retrieve client info by clientId for validation
            var client =  await _clientCacheService.GetClientByClientId(refreshTokenRequestDTO.ClientId);
            if (client == null)
            {
                _logger.LogInformation($"[User Service | Refresh Token: The client, {refreshTokenRequestDTO.ClientId}, is not found]");
                // Client invalid or inactive; reject refresh
                return null;
            }
            // Look up the refresh token in database, including related user and roles for new token generation
        
            var existingToken = _tokenService.GetExistingToken(refreshTokenRequestDTO.RefreshToken, client.Id);
            
            // Validate refresh token existence, revocation status, and expiration
            if (existingToken == null || existingToken.IsRevoked || existingToken.Expires <= DateTime.UtcNow)
            {
                _logger.LogInformation("[User service | Refresh Token: existing token is null or is revoked or expired.]");
                return null; // Invalid refresh token
            }
            // Revoke old refresh token immediately to prevent reuse
            _tokenService.RevokeToken(existingToken);

            var user = existingToken.User;
            if (user == null) 
            {
                _logger.LogInformation("[User service | Refresh Token: user in existing token is null]");
                return null;
            }
            var userRoleName = RoleMappingEnum2String(user.Role);
            var roles = new List<string>();
            if (!string.IsNullOrEmpty(userRoleName)) roles.Add(userRoleName);
            // Generate a new access token with fresh JWT ID and client info
            var accessToken = _tokenService.GenerateAccessToken(user, roles, out string newJwtId, client);
            if (accessToken == null || accessToken == "")
            {
                _logger.LogInformation("[User service | Refresh Token: access Token is generated unsuccessfully.]");
                return null;
            }
            // Generate a new refresh token linked to the new JWT ID
            var newRefreshToken = _tokenService.GenerateRefreshToken(ipAddress, newJwtId, client, user.Id);
            if (newRefreshToken == null)
            {
                 _logger.LogInformation("[User service | Refresh Token: refresh Token is generated unsuccessfully.]");

                return null;
            }
            // Store the new refresh token in the database
            _tokenService.AddRefreshTokens(newRefreshToken);
            // Read access token expiration duration from config or default to 15 minutes
            var accessTokenExpiryMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;
            // Return the new tokens and expiry info
            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes)
            };
        }

        // Revokes an existing refresh token to prevent further use
        public bool RevokeRefreshToken(string refreshToken)
        {
            var existingToken = _tokenService.GetExistingToken(refreshToken);
            // Return false if token not found or already revoked
            if (existingToken == null || existingToken.IsRevoked)
                return false;
            // Mark token as revoked and record revocation time
            _tokenService.RevokeToken(existingToken);
            return true; // Indicate successful revocation
        }
    }
}