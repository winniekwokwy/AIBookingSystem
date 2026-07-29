using AIBookingSystem.Repositories;
using AIBookingSystem.Services;
using AIBookingSystem.Enums;
using AIBookingSystem.DTO;
using AIBookingSystem.Models;

using Moq;
using Bogus;
using System.Net.Cache;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Tests.Services

{
    public class UserServiceUnitTests
    {
        private readonly UserService _userService;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IClientCacheService> _mockClientCacheService;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly IConfiguration _configuration;

        public UserServiceUnitTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockClientCacheService = new Mock<IClientCacheService>();
            _mockTokenService = new Mock<ITokenService>();
            _configuration = new ConfigurationBuilder()
                                    .AddInMemoryCollection(new Dictionary<string, string?>
                                    {
                                        ["JwtSettings:AccessTokenExpirationMinutes"] = "15"
                                    })
                                    .Build();
            _userService = new UserService(_mockUserRepo.Object, _mockTokenService.Object, _mockClientCacheService.Object, _configuration);
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
            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string role = "Admin";
            string status = "Active";

            var result = _userService.MapUser2DTO(new User
                        {
                            Id = 1,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
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
            string username = "maynicolaos";
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
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
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
            string username = "maynicolaos";
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
            string username = "maynicolaos";
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
            string password = "App13M@ng0";

            List<User> users = new List<User>
            {
            new User(){ Id = 1, Name = "Apple Mango", UserName = "applemango", PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), Role = UserRoles.Admin, Status = UserStatus.Active},
            new User() { Id = 2, Name = "Ben Smith", UserName = "bensmith", PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), Role = UserRoles.User, Status = UserStatus.Active}
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
            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string role = "Admin";
            string status = "Active";

            _mockUserRepo.Setup(repo => repo.GetUserbyID(id))
                        .Returns(new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
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
        public void GetUserByUsername_ExistingUserWithSmallLetter_ReturnUser()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string role = "Admin";
            string status = "Active";

            _mockUserRepo.Setup(repo => repo.GetUserbyUsername(username))
                        .Returns(new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        });
            var result = Assert.IsType<UserDTO>(_userService.GetUserbyUsername(username));
            Assert.NotNull(result);
            var userName = result.UserName;

            Assert.NotNull(userName);
            Assert.Equal(id, result.Id);
            Assert.Equal(name, result.Name);
            Assert.Equal(username.ToLower(), userName!);
            Assert.Equal(role, result.Role);
            Assert.Equal(status, result.Status);
        }

        [Fact]
        public void GetUserByUsername_ExistingUserWithCapitalLetter_ReturnUser()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "MAYNICOLAOS";
            string password = "M@yNic01@0s";
            string role = "Admin";
            string status = "Active";

            _mockUserRepo.Setup(repo => repo.GetUserbyUsername(username.ToLower()))
                        .Returns(new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username.ToLower(),
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        });
            var result = Assert.IsType<UserDTO>(_userService.GetUserbyUsername(username.ToLower()));
            Assert.NotNull(result);
            var userName = result.UserName;

            Assert.NotNull(userName);
            Assert.Equal(id, result.Id);
            Assert.Equal(name, result.Name);
            Assert.Equal(username.ToLower(), userName!);
            Assert.Equal(role, result.Role);
            Assert.Equal(status, result.Status);
        }

        [Fact]
        public void GetUserByUsername_UserNotFound_ReturnNull()
        {
            string username = "applemango";
            _mockUserRepo.Setup(repo => repo.GetUserbyUsername(username))
                        .Returns((User?)null);

            var result = _userService.GetUserbyUsername(username);

            Assert.Null(result);
        }

        [Fact]
        public void IsUserValid_ValidUserInputsSmallLetter_ReturnTrue()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "maynicolaos";
            string password = "M@yNic01@0s";

            _mockUserRepo.Setup(repo => repo.GetUserbyID(id))
                        .Returns(new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        });
            _mockUserRepo.Setup(repo => repo.GetUserbyUsername(username))
                        .Returns(new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        });
            var result = _userService.IsUserValid(id, username);

            Assert.True(result);
        }

        [Fact]
        public void IsUserValid_ValidUserInputsCapitalLetter_ReturnTrue()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "MAYNICOLAOS";
            string password = "M@yNic01@0s";

            _mockUserRepo.Setup(repo => repo.GetUserbyID(id))
                        .Returns(new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username.ToLower(),
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        });
            _mockUserRepo.Setup(repo => repo.GetUserbyUsername(username.ToLower()))
                        .Returns(new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username.ToLower(),
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        });
            var result = _userService.IsUserValid(id, username);

            Assert.True(result);
        }

        [Fact]
        public void IsUserValid_InvalidId_ReturnTrue()
        {
            int id = -1;
            string username = "maynicolaos";

            var result = _userService.IsUserValid(id, username);

            Assert.False(result);
        }

        [Fact]
        public void IsUserValid_UsernameNull_ReturnTrue()
        {
            int id = 1;
            string? username = null;

            var result = _userService.IsUserValid(id, username!);

            Assert.False(result);
        }

        [Fact]
        public void IsUserValid_UsernameEmpty_ReturnTrue()
        {
            int id = 1;
            string username = "";

            var result = _userService.IsUserValid(id, username);

            Assert.False(result);
        }

        [Fact]
        public void IsUserValid_GetUserbyIDReturnNull_ReturnTrue()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "maynicolaos";
            string password = "M@yNic01@0s";

            _mockUserRepo.Setup(repo => repo.GetUserbyID(id))
                        .Returns((User?)null);
            _mockUserRepo.Setup(repo => repo.GetUserbyUsername(username))
                        .Returns(new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        });
            var result = _userService.IsUserValid(id, username);

            Assert.False(result);
        }

        [Fact]
        public void IsUserValid_GetUserbyUsernameReturnNull_ReturnTrue()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "maynicolaos";
            string password = "M@yNic01@0s";

            _mockUserRepo.Setup(repo => repo.GetUserbyID(id))
                        .Returns(new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        });
            _mockUserRepo.Setup(repo => repo.GetUserbyUsername(username))
                        .Returns((User?)null);
            var result = _userService.IsUserValid(id, username);

            Assert.False(result);
        }

        [Fact]
        public void IsUserValid_ValidIdNotMatchWithUsername_ReturnFalse()
        {
            int id1 = 1;
            string name1 = "May Nicolaos";
            string username1 = "maynicolaos";
            string password = "M@yNic01@0s";

            int id2 = 2;
            string name2 = "Happy Person";
            string username2 = "happyperson";

            _mockUserRepo.Setup(repo => repo.GetUserbyID(id1))
                        .Returns(new User
                        {
                            Id = id1,
                            Name = name1,
                            UserName = username1,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        });
            _mockUserRepo.Setup(repo => repo.GetUserbyUsername(username2))
                        .Returns(new User
                        {
                            Id = id2,
                            Name = name2,
                            UserName = username2,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        });
            var result = _userService.IsUserValid(id1, username2);

            Assert.False(result);
        }

        [Fact]
        public async Task AuthenticateUser_ValidParameters_ReturnAuthResponseDTO()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string clientId = "client-app-one";
            
            Client client = new Client()
                            {
                                Id = 1,
                                ClientId = clientId, // Unique client identifier used in JWT tokens
                                Name = "Demo Client Application One",
                                ClientSecret = "fPXxcJw8TW5sA+S4rl4tIPcKk+oXAqoRBo+1s2yjUS4=", // Base64-encoded secret key
                                ClientURL = "https://clientappone.example.com", // Used as Audience in JWT validation
                                IsActive = true // Active client flag
                            };

            UserLoginDTO loginDTO = new UserLoginDTO
            {
                UserName = username,
                Password = password,
                ClientId = clientId
            };

            User user = new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        };

            var faker = new Faker();
            string ipAddress = faker.Internet.Ipv6();
            
            List<string> roles = Enum.GetNames(typeof(UserRoles)).ToList();

            string accessToken = "fake-refresh-token";
            var refreshToken = new RefreshToken
                                {
                                    Token = accessToken,
                                    Expires = DateTime.UtcNow.AddDays(7),
                                    IsRevoked = false
                                };

            var accessTokenExpiryMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;

            string jwtId = "test-jwt-id";

            _mockUserRepo.Setup(r => r.GetUserbyUsername(loginDTO.UserName))
                        .Returns(user);

            _mockClientCacheService.Setup(s => s.GetClientByClientId(loginDTO.ClientId))
                                .ReturnsAsync(client);
            _mockTokenService.Setup(s => s.GenerateAccessToken(user, roles, out jwtId, client))
                                .Returns(accessToken);
            _mockTokenService.Setup(s => s.GenerateRefreshToken(ipAddress, jwtId, client, user.Id))
                                .Returns(refreshToken);
            _mockTokenService.Setup(s => s.AddRefreshTokens(refreshToken));
            
            var result = await _userService.AuthenticateUser(loginDTO, ipAddress);
            _mockUserRepo.Verify(r => r.GetUserbyUsername(loginDTO.UserName), Times.Once);
            _mockClientCacheService.Verify(s => s.GetClientByClientId(client.ClientId), Times.Once);
            _mockTokenService.Verify(s => s.GenerateAccessToken(user, roles, out jwtId, client), Times.Once);
            _mockTokenService.Verify(s => s.GenerateRefreshToken(ipAddress, jwtId, client, user.Id), Times.Once);
            _mockTokenService.Verify(s => s.AddRefreshTokens(refreshToken), Times.Once);
            
            Assert.NotNull(result);
            Assert.Equal(accessToken, result.AccessToken);
            Assert.Equal(refreshToken.Token, result.RefreshToken);
            Assert.True(result.AccessTokenExpiresAt > DateTime.UtcNow);
            _mockTokenService.Verify(x => x.AddRefreshTokens(refreshToken), Times.Once);
        }

       [Fact]
        public async Task AuthenticateUser_PasswordNotMatch_ReturnNull()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string clientId = "client-app-one";
            
            Client client = new Client()
                            {
                                Id = 1,
                                ClientId = clientId, // Unique client identifier used in JWT tokens
                                Name = "Demo Client Application One",
                                ClientSecret = "fPXxcJw8TW5sA+S4rl4tIPcKk+oXAqoRBo+1s2yjUS4=", // Base64-encoded secret key
                                ClientURL = "https://clientappone.example.com", // Used as Audience in JWT validation
                                IsActive = true // Active client flag
                            };

            UserLoginDTO loginDTO = new UserLoginDTO
            {
                UserName = username,
                Password = password,
                ClientId = clientId
            };

            User user = new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password+"!"),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        };

            var faker = new Faker();
            string ipAddress = faker.Internet.Ipv6();
            
            List<string> roles = Enum.GetNames(typeof(UserRoles)).ToList();

            string accessToken = "fake-refresh-token";
            var refreshToken = new RefreshToken
                                {
                                    Token = accessToken,
                                    Expires = DateTime.UtcNow.AddDays(7),
                                    IsRevoked = false
                                };

            var accessTokenExpiryMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;

            string jwtId = "test-jwt-id";

            _mockUserRepo.Setup(r => r.GetUserbyUsername(loginDTO.UserName))
                        .Returns(user);

            _mockClientCacheService.Setup(s => s.GetClientByClientId(loginDTO.ClientId))
                                .ReturnsAsync(client);
            _mockTokenService.Setup(s => s.GenerateAccessToken(user, roles, out jwtId, client))
                                .Returns(accessToken);
            _mockTokenService.Setup(s => s.GenerateRefreshToken(ipAddress, jwtId, client, user.Id))
                                .Returns(refreshToken);
            _mockTokenService.Setup(s => s.AddRefreshTokens(refreshToken));
            
            var result = await _userService.AuthenticateUser(loginDTO, ipAddress);
            
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUser_RepoAuthenticateUserFailed_ReturnNull()
        {
            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string clientId = "client-app-one";
            
            UserLoginDTO loginDTO = new UserLoginDTO
            {
                UserName = username,
                Password = password,
                ClientId = clientId
            };

            var faker = new Faker();
            string ipAddress = faker.Internet.Ipv6();
     
            _mockUserRepo.Setup(r => r.GetUserbyUsername(loginDTO.UserName))
                        .Returns((User?)null);

            var result = await _userService.AuthenticateUser(loginDTO, ipAddress);
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUser_InvalidLoginDTO_ReturnNull()
        {
            UserLoginDTO? loginDTO = null!;
            
            var faker = new Faker();
            string ipAddress = faker.Internet.Ipv6();
            
            var result = await _userService.AuthenticateUser(loginDTO, ipAddress);
            Assert.Null(result);
        }        

        [Fact]
        public async Task AuthenticateUser_IpAddressNull_ReturnNull()
        {

            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string clientId = "client-app-one";
            
            UserLoginDTO loginDTO = new UserLoginDTO
            {
                UserName = username,
                Password = password,
                ClientId = clientId
            };

            string? ipAddress = null!;
                      
            var result = await _userService.AuthenticateUser(loginDTO, ipAddress);
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUser_IpAddressEmpty_ReturnNull()
        {

            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string clientId = "client-app-one";
            
            UserLoginDTO loginDTO = new UserLoginDTO
            {
                UserName = username,
                Password = password,
                ClientId = clientId
            };

            string? ipAddress = "";
                      
            var result = await _userService.AuthenticateUser(loginDTO, ipAddress);
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUser_InvalidIpAddress_ReturnNull()
        {

            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string clientId = "client-app-one";
            
            UserLoginDTO loginDTO = new UserLoginDTO
            {
                UserName = username,
                Password = password,
                ClientId = clientId
            };

            string ipAddress = "10.fdsf.122.fsfds";
                      
            var result = await _userService.AuthenticateUser(loginDTO, ipAddress);
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUser_GetClientFailed_ReturnAuthResponseDTO()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string clientId = "client-app-one";
            
            UserLoginDTO loginDTO = new UserLoginDTO
            {
                UserName = username,
                Password = password,
                ClientId = clientId
            };

            User user = new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        };

            var faker = new Faker();
            string ipAddress = faker.Internet.Ipv6();
            
            _mockUserRepo.Setup(r => r.GetUserbyUsername(loginDTO.UserName))
                        .Returns(user);

            _mockClientCacheService.Setup(s => s.GetClientByClientId(clientId))
                                .ReturnsAsync((Client?)null);
           
            var result = await _userService.AuthenticateUser(loginDTO, ipAddress);
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUser_GenerateAccessTokenFailed_ReturnNull()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string clientId = "client-app-one";
            
            Client client = new Client()
                            {
                                Id = 1,
                                ClientId = clientId, // Unique client identifier used in JWT tokens
                                Name = "Demo Client Application One",
                                ClientSecret = "fPXxcJw8TW5sA+S4rl4tIPcKk+oXAqoRBo+1s2yjUS4=", // Base64-encoded secret key
                                ClientURL = "https://clientappone.example.com", // Used as Audience in JWT validation
                                IsActive = true // Active client flag
                            };

            UserLoginDTO loginDTO = new UserLoginDTO
            {
                UserName = username,
                Password = password,
                ClientId = clientId
            };

            User user = new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        };

            var faker = new Faker();
            string ipAddress = faker.Internet.Ipv6();
            
            List<string> roles = Enum.GetNames(typeof(UserRoles)).ToList();

            string accessToken = "fake-refresh-token";
            var refreshToken = new RefreshToken
                                {
                                    Token = accessToken,
                                    Expires = DateTime.UtcNow.AddDays(7),
                                    IsRevoked = false
                                };

            var accessTokenExpiryMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;

            string jwtId = "test-jwt-id";

            _mockUserRepo.Setup(r => r.GetUserbyUsername(loginDTO.UserName))
                        .Returns(user);

            _mockClientCacheService.Setup(s => s.GetClientByClientId(client.ClientId))
                                .ReturnsAsync(client);
            _mockTokenService.Setup(s => s.GenerateAccessToken(user, roles, out jwtId, client))
                                .Returns((string?)null!);
            
            var result = await _userService.AuthenticateUser(loginDTO, ipAddress);
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUser_GenerateRefreshTokenFailed_ReturnNull()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string clientId = "client-app-one";
            
            Client client = new Client()
                            {
                                Id = 1,
                                ClientId = clientId, // Unique client identifier used in JWT tokens
                                Name = "Demo Client Application One",
                                ClientSecret = "fPXxcJw8TW5sA+S4rl4tIPcKk+oXAqoRBo+1s2yjUS4=", // Base64-encoded secret key
                                ClientURL = "https://clientappone.example.com", // Used as Audience in JWT validation
                                IsActive = true // Active client flag
                            };

            UserLoginDTO loginDTO = new UserLoginDTO
            {
                UserName = username,
                Password = password,
                ClientId = clientId
            };

            User user = new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        };

            var faker = new Faker();
            string ipAddress = faker.Internet.Ipv6();
            
            List<string> roles = Enum.GetNames(typeof(UserRoles)).ToList();

            string accessToken = "fake-refresh-token";
            var refreshToken = new RefreshToken
                                {
                                    Token = accessToken,
                                    Expires = DateTime.UtcNow.AddDays(7),
                                    IsRevoked = false
                                };

            var accessTokenExpiryMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;

            string jwtId = "test-jwt-id";

            _mockUserRepo.Setup(r => r.GetUserbyUsername(loginDTO.UserName))
                        .Returns(user);

            _mockClientCacheService.Setup(s => s.GetClientByClientId(client.ClientId))
                                .ReturnsAsync(client);
            _mockTokenService.Setup(s => s.GenerateAccessToken(user, roles, out jwtId, client))
                                .Returns(accessToken);
            _mockTokenService.Setup(s => s.GenerateRefreshToken(ipAddress, jwtId, client, user.Id))
                                .Returns((RefreshToken?)null!);
            
            var result = await _userService.AuthenticateUser(loginDTO, ipAddress);
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshToken_ValidParameters_ReturnAuthResponseDTO()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string clientId = "client-app-one";
            
            Client client = new Client()
                            {
                                Id = 1,
                                ClientId = clientId, // Unique client identifier used in JWT tokens
                                Name = "Demo Client Application One",
                                ClientSecret = "fPXxcJw8TW5sA+S4rl4tIPcKk+oXAqoRBo+1s2yjUS4=", // Base64-encoded secret key
                                ClientURL = "https://clientappone.example.com", // Used as Audience in JWT validation
                                IsActive = true // Active client flag
                            };

            User user = new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        };

            var faker = new Faker();
            string ipAddress = faker.Internet.Ipv6();
            
            List<string> roles = Enum.GetNames(typeof(UserRoles)).ToList();

            string accessToken = "fake-access-token";
            string refreshToken = "fake-refresh-token";

            var existingToken = new RefreshToken
                                {
                                    Token = refreshToken,
                                    Expires = DateTime.UtcNow.AddDays(7),
                                    IsRevoked = false,
                                    User = user
                                };
            var newRefreshToken = new RefreshToken
                    {
                        Token = accessToken,
                        Expires = DateTime.UtcNow.AddDays(7),
                        IsRevoked = false,
                    };

            var accessTokenExpiryMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;

            string jwtId = "test-jwt-id";

            _mockClientCacheService.Setup(s => s.GetClientByClientId(client.ClientId))
                                .ReturnsAsync(client);
            _mockTokenService.Setup(s => s.GetExistingToken(refreshToken, id))
                                .Returns(existingToken);
            _mockTokenService.Setup(s => s.GenerateAccessToken(user, roles, out jwtId, client))
                                .Returns(accessToken);
            _mockTokenService.Setup(s => s.GenerateRefreshToken(ipAddress, jwtId, client, user.Id))
                                .Returns(newRefreshToken);
            _mockTokenService.Setup(s => s.AddRefreshTokens(newRefreshToken));
            
            var result = await _userService.RefreshToken(refreshToken, clientId, ipAddress);
            Assert.NotNull(result);
            Assert.Equal(accessToken, result.AccessToken);
            Assert.Equal(newRefreshToken.Token, result.RefreshToken);
            Assert.True(result.AccessTokenExpiresAt > DateTime.UtcNow);
            _mockTokenService.Verify(x => x.AddRefreshTokens(newRefreshToken), Times.Once);            
        }

        [Fact]
        public async Task RefreshToken_NullRefreshToken_ReturnNull()
        {
            string clientId = "client-app-one";
            
            var faker = new Faker();
            string ipAddress = faker.Internet.Ipv6();
            
            string? refreshToken = null!;
       
            var result = await _userService.RefreshToken(refreshToken, clientId, ipAddress);
            Assert.Null(result);          
        }

        [Fact]
        public async Task RefreshToken_EmptyRefreshToken_ReturnNull()
        {
            string clientId = "client-app-one";
            
            var faker = new Faker();
            string ipAddress = faker.Internet.Ipv6();
            
            string? refreshToken = ""!;
       
            var result = await _userService.RefreshToken(refreshToken, clientId, ipAddress);
            Assert.Null(result);          
        }

        [Fact]
        public async Task RefreshToken_GetClientFailed_ReturnNull()
        {
            string clientId = "client-app-one";
            
            var faker = new Faker();
            string ipAddress = faker.Internet.Ipv6();

            string refreshToken = "fake-refresh-token";

            _mockClientCacheService.Setup(s => s.GetClientByClientId(clientId))
                                .ReturnsAsync((Client?)null!);
     
            var result = await _userService.RefreshToken(refreshToken, clientId, ipAddress);
            Assert.Null(result);           
        }

        [Fact]
        public async Task RefreshToken_GetExistingTokenFailed_ReturnAuthResponseDTO()
        {
            int id = 1;
            string clientId = "client-app-one";
            
            Client client = new Client()
                            {
                                Id = 1,
                                ClientId = clientId, // Unique client identifier used in JWT tokens
                                Name = "Demo Client Application One",
                                ClientSecret = "fPXxcJw8TW5sA+S4rl4tIPcKk+oXAqoRBo+1s2yjUS4=", // Base64-encoded secret key
                                ClientURL = "https://clientappone.example.com", // Used as Audience in JWT validation
                                IsActive = true // Active client flag
                            };

            var faker = new Faker();
            string ipAddress = faker.Internet.Ipv6();
            
            string refreshToken = "fake-refresh-token";

            _mockClientCacheService.Setup(s => s.GetClientByClientId(clientId))
                                .ReturnsAsync(client);
            _mockTokenService.Setup(s => s.GetExistingToken(refreshToken, id))
                                .Returns((RefreshToken?)null!);
         
            var result = await _userService.RefreshToken(refreshToken, clientId, ipAddress);
            Assert.Null(result);         
        }

        [Fact]
        public async Task RefreshToken_GenerateAccessTokenFailed_ReturnNull()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string clientId = "client-app-one";
            
            Client client = new Client()
                            {
                                Id = 1,
                                ClientId = clientId, // Unique client identifier used in JWT tokens
                                Name = "Demo Client Application One",
                                ClientSecret = "fPXxcJw8TW5sA+S4rl4tIPcKk+oXAqoRBo+1s2yjUS4=", // Base64-encoded secret key
                                ClientURL = "https://clientappone.example.com", // Used as Audience in JWT validation
                                IsActive = true // Active client flag
                            };

            User user = new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        };

            var faker = new Faker();
            string ipAddress = faker.Internet.Ipv6();
            
            List<string> roles = Enum.GetNames(typeof(UserRoles)).ToList();

            string refreshToken = "fake-refresh-token";

            var existingToken = new RefreshToken
                                {
                                    Token = refreshToken,
                                    Expires = DateTime.UtcNow.AddDays(7),
                                    IsRevoked = false,
                                    User = user
                                };

            string jwtId = "test-jwt-id";

            _mockClientCacheService.Setup(s => s.GetClientByClientId(clientId))
                                .ReturnsAsync(client);
            _mockTokenService.Setup(s => s.GetExistingToken(refreshToken, id))
                                .Returns(existingToken);
            _mockTokenService.Setup(s => s.GenerateAccessToken(user, roles, out jwtId, client))
                                .Returns((string?)null!);

            var result = await _userService.RefreshToken(refreshToken, clientId, ipAddress);
            Assert.Null(result);     
        }

        [Fact]
        public async Task RefreshToken_GenerateRefreshTokenFailed_ReturnAuthResponseDTO()
        {
            int id = 1;
            string name = "May Nicolaos";
            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string clientId = "client-app-one";
            
            Client client = new Client()
                            {
                                Id = 1,
                                ClientId = clientId, // Unique client identifier used in JWT tokens
                                Name = "Demo Client Application One",
                                ClientSecret = "fPXxcJw8TW5sA+S4rl4tIPcKk+oXAqoRBo+1s2yjUS4=", // Base64-encoded secret key
                                ClientURL = "https://clientappone.example.com", // Used as Audience in JWT validation
                                IsActive = true // Active client flag
                            };

            User user = new User
                        {
                            Id = id,
                            Name = name,
                            UserName = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            Role = UserRoles.Admin,
                            Status = UserStatus.Active
                        };

            var faker = new Faker();
            string ipAddress = faker.Internet.Ipv6();
            
            List<string> roles = Enum.GetNames(typeof(UserRoles)).ToList();

            string accessToken = "fake-access-token";
            string refreshToken = "fake-refresh-token";

            var existingToken = new RefreshToken
                                {
                                    Token = refreshToken,
                                    Expires = DateTime.UtcNow.AddDays(7),
                                    IsRevoked = false,
                                    User = user
                                };

            var accessTokenExpiryMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;

            string jwtId = "test-jwt-id";

            _mockClientCacheService.Setup(s => s.GetClientByClientId(client.ClientId))
                                .ReturnsAsync(client);
            _mockTokenService.Setup(s => s.GetExistingToken(refreshToken, id))
                                .Returns(existingToken);
            _mockTokenService.Setup(s => s.GenerateAccessToken(user, roles, out jwtId, client))
                                .Returns(accessToken);
            _mockTokenService.Setup(s => s.GenerateRefreshToken(ipAddress, jwtId, client, user.Id))
                                .Returns((RefreshToken?)null!);
            
            var result = await _userService.RefreshToken(refreshToken, clientId, ipAddress);
            Assert.Null(result);         
        }
    }
}
