using AIBookingSystem.Repositories;
using AIBookingSystem.Services;
using AIBookingSystem.Enums;
using AIBookingSystem.DTO;

using Moq;

namespace WebAPI.Tests.Services

{
    public class UserServiceUnitTests
    {
        private readonly UserService _userService;
        private readonly Mock<IUserRepository> _mockUserRepo;

        public UserServiceUnitTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepo.Object);
        }
        
        [Fact]
        public void IsRoleValid_ValidEnumAdminRole_True()
        {
            var result = _userService.IsRoleValid(UserRoles.Admin);
            Assert.True(result);
        }

        [Fact]
        public void IsRoleValid_ValidEnumUserRole_True()
        {
            var result = _userService.IsRoleValid(UserRoles.User);
            Assert.True(result);
        }

        [Fact]
        public void IsRoleValid_InvalidEnumRole_False()
        {
            var invalidRole = (UserRoles) (-1);
            var result = _userService.IsRoleValid(invalidRole);
            Assert.False(result);
        }

        [Fact]
        public void IsRoleValid_ValidRole_True()
        {
            var result = _userService.IsRoleValid("Admin");
            Assert.True(result);
        }

        [Fact]
        public void IsRoleValid_InvalidRole_False()
        {
            var result = _userService.IsRoleValid("Admi");
            Assert.False(result);
        }

        [Fact]
        public void IsStatusValid_ActiveStatus_True()
        {
            var result = _userService.IsStatusValid(UserStatus.Active);
            Assert.True(result);
        }

        [Fact]
        public void IsStatusValid_InactiveStatus_True()
        {
            var result = _userService.IsStatusValid(UserStatus.Inactive);
            Assert.True(result);
        }

        [Fact]
        public void IsStatusValid_InvalidStatus_False()
        {
            var invalidStatus = (UserStatus) (-1);
            var result = _userService.IsStatusValid(invalidStatus);
            Assert.False(result);
        }

        [Fact]
        public void RoleMappingString2Enum_ValidAdminRole_ReturnAdminEnum()
        {
            var result = _userService.RoleMappingString2Enum("Admin");
            Assert.Equal(UserRoles.Admin, result);
            
        }

        [Fact]
        public void RoleMappingString2Enum_ValidUserRole_ReturnUserEnum()
        {
            var result = _userService.RoleMappingString2Enum("User");
            Assert.Equal(UserRoles.User, result);
            
        }

        [Fact]
        public void RoleMappingString2Enum_InvalidRole_ReturnInvalidEnum()
        {
            var result = _userService.RoleMappingString2Enum("Admi");
            Assert.Equal((UserRoles) (-1), result);
            
        }
        
        [Fact]
        public void RoleMappingEnum2String_ValidAdminRole_ReturnAdminString()
        {
            var result = _userService.RoleMappingEnum2String(UserRoles.Admin);
            Assert.Equal("Admin", result);
            
        }

        [Fact]
        public void RoleMappingEnum2String_ValidUserRole_ReturnUserString()
        {
            var result = _userService.RoleMappingEnum2String(UserRoles.User);
            Assert.Equal("User", result);
            
        }

        [Fact]
        public void RoleMappingEnum2String_InvalidRole_ReturnInvalidString()
        {
            var result = _userService.RoleMappingEnum2String((UserRoles) (-1));
            Assert.Null(result);
        }

        [Fact]
        public void StatusMappingString2Enum_ValidActiveStatus_ReturnActiveEnum()
        {
            var result = _userService.StatusMappingString2Enum("Active");
            Assert.Equal(UserStatus.Active, result);
            
        }

        [Fact]
        public void StatusMappingString2Enum_ValidInActiveStatus_ReturnInactiveEnum()
        {
            var result = _userService.StatusMappingString2Enum("Inactive");
            Assert.Equal(UserStatus.Inactive, result);
            
        }

        [Fact]
        public void StatusMappingString2Enum_InvalidStatus_ReturnInvalidEnum()
        {
            var result = _userService.StatusMappingString2Enum("Activ");
            Assert.Equal((UserStatus) (-1), result);
            
        }

        [Fact]
        public void StatusMappingEnum2String_ActiveStatus_ReturnActiveString()
        {
            var result = _userService.StatusMappingEnum2String(UserStatus.Active);
            Assert.Equal("Active", result);
            
        }

        [Fact]
        public void StatusMappingEnum2String_InactiveStatus_ReturnInactiveString()
        {
            var result = _userService.StatusMappingEnum2String(UserStatus.Inactive);
            Assert.Equal("Inactive", result);
            
        }

        [Fact]
        public void StatusMappingEnum2String_InvalidStatus_ReturnNull()
        {
            var result = _userService.StatusMappingEnum2String((UserStatus) (-1));
            Assert.Null(result);
        }

        [Fact]
        public void MapUser2DTO_ValidUser_ReturnUserDTO()
        {
            string name = "May Nicolaos";
            string username = "MayNicolaos";
            string password = "M@yNic01@0s";
            string role = "Admin";
            string status = "Active";

            var result = _userService.MapUser2DTO(new User
                        {
                            Id = 1,
                            Name = name,
                            UserName = username,
                            Password = password,
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        });
            Assert.NotNull(result);
            Assert.Equal(name, result.Name);
            Assert.Equal(username, result.UserName);
            Assert.Equal(role, result.Role);
            Assert.Equal(status, result.Status);    
        }

        [Fact]
        public void MapUser2DTO_NullUser_ReturnNull()
        {

            var result = _userService.MapUser2DTO(null!);

            Assert.Null(result);

        }

        [Fact]
        public void CreateUser_ValidUserCreateDTO_ReturnUserDTO()
        {
            string name = "May Nicolaos";
            string username = "MayNicolaos";
            string password = "M@yNic01@0s";
            string role = "Admin";
            string status = "Active";


            _mockUserRepo.Setup(repo => repo.UsernameInUse(username))
                        .Returns(false);

            _mockUserRepo.Setup(repo => repo.CreateUser(It.IsAny<User>()))
                        .Returns(new User
                        {
                            Id = 1,
                            Name = name,
                            UserName = username,
                            Password = password,
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        });

            var result = _userService.CreateUser(new UserCreateDTO
            {
            Name = name,
            UserName = username,
            Password = password,
            Role = role,
            Status = status
            });

            Assert.NotNull(result);
            Assert.Equal(name, result.Name);
            Assert.Equal(username, result.UserName);
            Assert.Equal(role, result.Role);
            Assert.Equal(status, result.Status);
        }

        [Fact]
        public void CreateUser_NullUserCreateDTO_ReturnNull()
        {
            var result = _userService.CreateUser(null!);

            Assert.Null(result);
        }

        [Fact]
        public void CreateUser_ExistingUser_ReturnNull()
        {
            string name = "May Nicolaos";
            string username = "MayNicolaos";
            string password = "M@yNic01@0s";
            string role = "Admin";
            string status = "Active";

            _mockUserRepo.Setup(repo => repo.UsernameInUse(username))
                        .Returns(true);

            var result = _userService.CreateUser(new UserCreateDTO
                        {
                            Name = name,
                            UserName = username,
                            Password = password,
                            Role = role,
                            Status = status
                        });

            Assert.Null(result);
        }

        [Fact]
        public void CreateUser_MissingUsername_ReturnNull()
        {
            string name = "May Nicolaos";
            string username = null!;
            string password = "M@yNic01@0s";
            string role = "Admin";
            string status = "Active";

            var result = _userService.CreateUser(new UserCreateDTO
                        {
                            Name = name,
                            UserName = username,
                            Password = password,
                            Role = role,
                            Status = status
                        });

            Assert.Null(result);
        }

        [Fact]
        public void CreateUser_Failed2RetrievedUser_ReturnNull()
        {
            string name = "May Nicolaos";
            string username = "MayNicolaos";
            string password = "M@yNic01@0s";
            string role = "Admin";
            string status = "Active";


            _mockUserRepo.Setup(repo => repo.UsernameInUse(username))
                        .Returns(false);

            _mockUserRepo.Setup(repo => repo.CreateUser(It.IsAny<User>()))
                        .Returns((User?)null);

            var result = _userService.CreateUser(new UserCreateDTO
            {
            Name = name,
            UserName = username,
            Password = password,
            Role = role,
            Status = status
            });

            Assert.Null(result);
        }

        [Fact]
        public void ListUsers_WithData_ReturnUsers()
        {

            List<User> users = new List<User>
            {
            new User(){ Id = 1, Name = "Apple Mango", UserName = "AppleMango", Password = "App13M@ng0", Role = UserRoles.Admin, Status = UserStatus.Active},
            new User() { Id = 2, Name = "Ben Smith", UserName = "BenSmith", Password = "B3nSmith!", Role = UserRoles.User, Status = UserStatus.Active}
            };
            _mockUserRepo.Setup(repo => repo.ListUsers())
                        .Returns(users);

            var result = _userService.ListUsers();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void ListUsers_NoUsersInDB_ReturnEmptyList()
        {

            List<User> users = new List<User>{};
            _mockUserRepo.Setup(repo => repo.ListUsers())
                        .Returns(users);

            var result = _userService.ListUsers();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ListUsers_WhenRepoReturnNull_ReturnNull()
        {

            _mockUserRepo.Setup(repo => repo.ListUsers())
                        .Returns((List<User>?) null);

            var result = _userService.ListUsers();

            Assert.Null(result);
        }

        [Fact]
        public void GetUserByID_ValidId_ReturnUser()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "MayNicolaos";
            string password = "M@yNic01@0s";
            string role = "Admin";
            string status = "Active";

            _mockUserRepo.Setup(repo => repo.GetUserbyID(id))
                        .Returns(new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            Password = password,
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        });
            var result = _userService.GetUserbyID(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(name, result.Name);
            Assert.Equal(username, result.UserName);
            Assert.Equal(role, result.Role);
            Assert.Equal(status, result.Status);
        }

        [Fact]
        public void GetUserByID_IdNotFound_ReturnNull()
        {
            int id = -1;
            _mockUserRepo.Setup(repo => repo.GetUserbyID(id))
                        .Returns((User?)null);
            var result = _userService.GetUserbyID(id);

            Assert.Null(result);
        }

        [Fact]
        public void GetUserByUsername_ExistingUser_ReturnUser()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "MayNicolaos";
            string password = "M@yNic01@0s";
            string role = "Admin";
            string status = "Active";

            _mockUserRepo.Setup(repo => repo.GetUserbyUsername(username))
                        .Returns(new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            Password = password,
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        });
            var result = _userService.GetUserbyUsername(username);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(name, result.Name);
            Assert.Equal(username, result.UserName);
            Assert.Equal(role, result.Role);
            Assert.Equal(status, result.Status);
        }

        [Fact]
        public void GetUserByUsername_UserNotFound_ReturnNull()
        {
            string username = "AppleMango";
            _mockUserRepo.Setup(repo => repo.GetUserbyUsername(username))
                        .Returns((User?)null);

            var result = _userService.GetUserbyUsername(username);

            Assert.Null(result);
        }
    }
}
