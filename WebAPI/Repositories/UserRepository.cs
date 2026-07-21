using System.Reflection.Metadata.Ecma335;
using AIBookingSystem.Data;
using AIBookingSystem.DTO;
using AIBookingSystem.Enums;
using AIBookingSystem.Models;

namespace AIBookingSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RoomBookingDbContext _dBContext;
        public UserRepository(RoomBookingDbContext dBContext)
        {
            _dBContext = dBContext;
        }

        public bool UsernameInUse(string username)
        {
            if (username != null)
            {
                if (GetUserbyUsername(username.ToLower())!= null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsUserValid(int userId, string bookedBy)
        {
            if (userId <= 0)
            {
                return false;
            }
            else if (bookedBy == null || bookedBy == "") 
            {
                return false;
            }
            else
            {
                var userFound = _dBContext.Users.FirstOrDefault(u => u.UserName == bookedBy.ToLower());

                if (userFound != null)
                {
                    if (userFound.Id == userId)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        public IEnumerable<User>? ListUsers()
        {
            return _dBContext.Users
                .ToList();
        }
        
        public User? GetUserbyID(int id)
        {
            return _dBContext.Users.FirstOrDefault(u => u.Id == id);
        }

        public User? GetUserbyUsername(string userName)
        {

            if (userName != null)
            {
                return _dBContext.Users.FirstOrDefault(u => u.UserName == userName.ToLower());
            }
            return null;
        }

        public User? CreateUser(User user)
        {    
            if (user != null) 
            {
                var username = user.UserName.ToLower();
                if (!UsernameInUse(username))
                {
                    _dBContext.Users.Add(user);
                    _dBContext.SaveChanges();

                    return _dBContext.Users.FirstOrDefault(u => u.UserName == username);
                }
            }
            return null;
        }

        public User? AuthenticateUser(UserLoginDTO loginDto)
        {
            // Retrieve user by email with roles eagerly loaded; only active users allowed
            var user = _dBContext.Users
                .FirstOrDefault(u => u.UserName == loginDto.UserName && u.Status == UserStatus.Active);

            // Verify user exists and password matches the stored hashed password
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
               return null; // Invalid credentials
            }
            // Retrieve client info by ClientId
            return user;
        }

        // Refreshes an expired access token using a valid refresh token and client ID
        public bool RefreshToken(string refreshToken, Client client)
        {
            // Look up the refresh token in database, including related user and roles for new token generation
            var existingToken = _dBContext.RefreshTokens
                .FirstOrDefault(rt => rt.Token == refreshToken && rt.ClientId == client.Id);
            // Validate refresh token existence, revocation status, and expiration
            if (existingToken == null || existingToken.IsRevoked || existingToken.Expires <= DateTime.UtcNow)
                return false; // Invalid refresh token
            // Revoke old refresh token immediately to prevent reuse
            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;
            _dBContext.SaveChanges();
            // var user = existingToken.User;
            // var roles = Enum.GetNames(typeof(UserRoles)).ToList();
            // // Generate a new access token with fresh JWT ID and client info
            // var accessToken = _tokenService.GenerateAccessToken(user, roles, out string newJwtId, client);
            // // Generate a new refresh token linked to the new JWT ID
            // var newRefreshToken = _tokenService.GenerateRefreshToken(ipAddress, newJwtId, client, user.Id);
            // // Store the new refresh token in the database
            // _dbContext.RefreshTokens.Add(newRefreshToken);
            // await _dbContext.SaveChangesAsync();
            // Read access token expiration duration from config or default to 15 minutes
            // var accessTokenExpiryMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;
            // // Return the new tokens and expiry info
            // return new AuthResponseDTO
            // {
            //     AccessToken = accessToken,
            //     RefreshToken = newRefreshToken.Token,
            //     AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes)
            // };
            return true;
        }
        // Revokes an existing refresh token to prevent further use
        public bool RevokeRefreshToken(string refreshToken, string ipAddress)
        {
//             // Look up the refresh token in the database
//             var existingToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
//             // Return false if token not found or already revoked
//             if (existingToken == null || existingToken.IsRevoked)
//                 return false;
//             // Mark token as revoked and record revocation time
//             existingToken.IsRevoked = true;
//             existingToken.RevokedAt = DateTime.UtcNow;
//             // Persist changes to database
//             await _dbContext.SaveChangesAsync();
            return true; // Indicate successful revocation
//         }
    }
    }
}