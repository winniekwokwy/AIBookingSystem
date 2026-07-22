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
            if (loginDto == null)
                return null;
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
    }
}