using System.Diagnostics;
using System.Runtime.CompilerServices;
using AIBookingSystem.DTO;
using AIBookingSystem.Enums;

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
                }
            }
            return null;
        }
        
        public UserStatus StatusMappingString2Enum(string status)
        {
            if (status == "Active")
            {
                return UserStatus.Active;
            }
            else
            {
                return UserStatus.Inactive;
            }
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
            if (role == "Admin")
            {
                return UserRoles.Admin;
            }
            else 
            {
                return UserRoles.User;
            }
        }
        public IEnumerable<UserDTO>? ListUsers()
        {
            return _dBContext.Users
                .ToList()
                .Select(u => new UserDTO
                    {
                        Id = u.Id,
                        Name = u.Name,
                        UserName = u.UserName,
                        Role = RoleMappingEnum2String(u.Role),
                        Status = StatusMappingEnum2String(u.Status)
                    }
                );
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
                Role = RoleMappingEnum2String(user.Role),
                Status = StatusMappingEnum2String(user.Status)
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
                Role = RoleMappingEnum2String(user.Role),
                Status = StatusMappingEnum2String(user.Status)
            };
        }

        public UserDTO? CreateUser(UserCreateDTO user)
        {
            var newUser = new User
                            {
                                Name = user.Name,
                                UserName = user.UserName,
                                Role = RoleMappingString2Enum(user.Role),
                                Password = user.Password,
                                Status = StatusMappingString2Enum(user.Status)
                            };
    
            _dBContext.Users.Add(newUser);
            _dBContext.SaveChanges();

            var addedUser = _dBContext.Users.FirstOrDefault(u => u.UserName == user.UserName);

            if (addedUser != null)
            {
                return new UserDTO
                {
                    Id = addedUser.Id,
                    Name = user.UserName,
                    UserName = user.UserName,
                    Role = user.Role,
                    Status = user.Status
                };
            }
            return null;
        }

    }
}