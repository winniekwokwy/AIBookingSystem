using AIBookingSystem.DTO;

namespace AIBookingSystem.Services
{
    public class UserService : IUserService
    {
        private readonly RoomBookingDbContext _dBContext;

        public UserService(RoomBookingDbContext dBContext)
        {
            _dBContext = dBContext;
        }

        public bool IsRoleValid(string role)
        {
            if ((role.ToLower() != "user") && role.ToLower() != "admin")
            {
                return false;
            }
            return true;
        }

        public bool UsernameExsited(string username)
        {
            if (_dBContext.Users.FirstOrDefault(u => u.UserName == username) != null)
            {
                return true;
            }
            return false;
        }

        public bool IsUserValid(UserCreateDTO userDTO)
        {
            var user = _dBContext.Users.FirstOrDefault(u => u.UserName == userDTO.CreatedBy);

            if (user != null)
            {
                if (user.Id == userDTO.UserId)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public IEnumerable<UserDTO>? ListUsers()
        {
            return _dBContext.Users
            .Select(u => new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                UserName = u.UserName,
                Role = u.Role,
                Status = u.Status
            })
            .ToList();
        }
        
        public UserDTO? GetUserbyID(int id)
        {
            var user = _dBContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return null;
            }

            return new UserDTO
            {
                Id = user.Id,
                Name = user.UserName,
                UserName = user.UserName,
                Role = user.Role,
                Status = user.Status
            };
        }

        public UserDTO? GetUserbyUsername(string userName)
        {

            var user = _dBContext.Users.FirstOrDefault(u => u.UserName == userName);

            if (user == null)
            {
                return null;
            }
            
            return new UserDTO
            {
                Id = user.Id,
                Name = user.UserName,
                UserName = user.UserName,
                Role = user.Role,
                Status = user.Status
            };
        }

        public UserDTO? CreateUser(UserCreateDTO user)
        {
            var newUser = new User
                            {
                                Name = user.Name,
                                UserName = user.UserName,
                                Role = user.Role,
                                Password = user.Password,
                                Status = "Active"
                            };

            _dBContext.Users.Add(newUser);
            _dBContext.SaveChanges();
            
            var addedUser = _dBContext.Users.FirstOrDefault(u => u.UserName == user.UserName);

            if (addedUser != null)
            {
                return new UserDTO
                {
                    Id = addedUser.Id,
                    Name = addedUser.UserName,
                    UserName = addedUser.UserName,
                    Role = addedUser.Role,
                    Status = addedUser.Status
                };
            }
            return null;
        }

    }
}