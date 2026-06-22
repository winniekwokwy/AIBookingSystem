using AIBookingSystem.Data;
using Microsoft.AspNetCore.Http.HttpResults;

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
                if (GetUserbyUsername(username)!= null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsUserValid(int userId, string createdBy)
        {
            if (userId <= 0)
            {
                return false;
            }
            else if (createdBy == null || createdBy == "") 
            {
                return false;
            }
            else
            {
                var userFound = _dBContext.Users.FirstOrDefault(u => u.UserName == createdBy);

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
            var user = _dBContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return null;
            }

            return user;
        }

        public User? GetUserbyUsername(string userName)
        {

            var user = _dBContext.Users.FirstOrDefault(u => u.UserName == userName);

            if (user == null)
            {
                return null;
            }
            
            return user;
        }

        public User? CreateUser(User user)
        {    
            if (user != null) 
            {
                if (!UsernameInUse(user.UserName))
                {
                    _dBContext.Users.Add(user);
                    _dBContext.SaveChanges();

                    var addedUser = _dBContext.Users.FirstOrDefault(u => u.UserName == user.UserName);

                    if (addedUser != null)
                    {
                        return addedUser;
                    }
                }
            }
            return null;
        }

    }
}