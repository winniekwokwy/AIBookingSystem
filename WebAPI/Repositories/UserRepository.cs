using AIBookingSystem.Data;

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
                if (!UsernameInUse(user.UserName))
                {
                    _dBContext.Users.Add(user);
                    _dBContext.SaveChanges();

                    return _dBContext.Users.FirstOrDefault(u => u.UserName == user.UserName.ToLower());
                }
            }
            return null;
        }

    }
}