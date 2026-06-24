using AIBookingSystem.DTO;
using AIBookingSystem.Enums;
using AIBookingSystem.Repositories;

namespace AIBookingSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
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
                    // .Select(u => new UserDTO
                    //     {
                    //         Id = u.Id,
                    //         Name = u.Name,
                    //         UserName = u.UserName,
                    //         Role = RoleMappingEnum2String(u.Role),
                    //         Status = StatusMappingEnum2String(u.Status)
                    //     }
                    // );
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

            var user = _userRepo.GetUserbyUsername(userName);

            if (user == null)
            {
                return null;
            }
            
            return MapUser2DTO(user);
        }

        public UserDTO? CreateUser(UserCreateDTO user)
        {
            if (user != null){
                if (user.UserName != null)
                {
                    if (!_userRepo.UsernameInUse(user.UserName))
                    {
                        var newUser = new User
                                        {
                                            Name = user.Name,
                                            UserName = user.UserName,
                                            Role = RoleMappingString2Enum(user.Role),
                                            Password = user.Password,
                                            Status = StatusMappingString2Enum(user.Status)
                                        };
                
                        var addedUser = _userRepo.CreateUser(newUser);

                        if (addedUser != null)
                        {
                            return MapUser2DTO(addedUser);
                        }
                    }
                }
            }
            return null;
        }

    }
}