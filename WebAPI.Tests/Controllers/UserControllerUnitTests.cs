using AIBookingSystem.Controllers;
using AIBookingSystem.Services;
using AIBookingSystem.DTO;
using AIBookingSystem.Enums;

using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace WebAPI.Tests.Controllers;

public class UserControllerUnitTests
{
    private readonly UserController _userController;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILogger<UserController>> _mockLogger;

    public UserControllerUnitTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockLogger = new Mock<ILogger<UserController>>();
        _userController = new UserController(_mockUserService.Object, _mockLogger.Object);
    }

    [Fact]
    public void ListUsers_FoundUsers_ReturnListOfUserDTO()
    {
        List<UserDTO> users = new List<UserDTO>
        {
           new UserDTO(){ Id = 1, Name = "Apple Mango", UserName = "AppleMango", Role = "Admin", Status = "Active"},
           new UserDTO(){ Id = 2, Name = "Ben Smith", UserName = "BenSmith", Role = "User", Status = "Active"}
        };
        _mockUserService.Setup(s => s.ListUsers())
                    .Returns(users);

        var result = _userController.ListUsers();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(users, okResult.Value);  
    }

    [Fact]
    public void ListUsers_NoUsers_ReturnNotFound()
    {
        _mockUserService.Setup(s => s.ListUsers())
                    .Returns((List<UserDTO>)null);

        var result = _userController.ListUsers();

        Assert.IsType<NotFoundObjectResult>(result.Result); 
    }

    [Fact]
    public void GetUserByID_ExistingUsers_ReturnOKWithUserDTO()
    {
        int id = 1;
        UserDTO user = new UserDTO(){ Id = id, Name = "Apple Mango", UserName = "AppleMango", Role = "Admin", Status = "Active"};
        
        _mockUserService.Setup(s => s.GetUserbyID(id))
                    .Returns(user);

        var result = _userController.GetUserByID(id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(user, okResult.Value);  
    }

    [Fact]
    public void GetUserByID_NonExistingUsers_ReturnNotFound()
    {
        int id = 100;
        _mockUserService.Setup(s => s.GetUserbyID(id))
                    .Returns((UserDTO)null);

        var result = _userController.GetUserByID(id);

        Assert.IsType<NotFoundObjectResult>(result.Result); 
    }

    [Fact]
    public void GetUserByID_UserwithInvalidID_ReturnNotFound()
    {
        int id = -1;

        var result = _userController.GetUserByID(id);

        Assert.IsType<NotFoundObjectResult>(result.Result); 
    }

    [Fact]
    public void GetUserByUsername_ExistingUsers_ReturnOKWithUserDTO()
    {
        string username = "AppleMango";
        UserDTO user = new UserDTO(){ Id = 1, Name = "Apple Mango", UserName = username, Role = "Admin", Status = "Active"};
        
        _mockUserService.Setup(s => s.GetUserbyUsername(username))
                    .Returns(user);

        var result = _userController.GetUserByUsername(username);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(user, okResult.Value);  
    }

    [Fact]
    public void GetUserByUsername_NullUsername_ReturnNotFound()
    {
        var result = _userController.GetUserByUsername(null);

        Assert.IsType<NotFoundObjectResult>(result.Result); 
    }

    [Fact]
    public void GetUserByUsername_NonExistingUsers_ReturnNotFound()
    {
        string username = "AppleMango";
        _mockUserService.Setup(s => s.GetUserbyUsername(username))
                    .Returns((UserDTO)null);

        var result = _userController.GetUserByUsername(username);

        Assert.IsType<NotFoundObjectResult>(result.Result); 
    }

    [Fact]
    public void CreateUser_ValidUserCreateDTO_ReturnUserDTO()

    {
        string name = "May Nicolaos";
        string username = "MayNicolaos";
        string password = "M@yNic01@0s";
        string role = "Admin";
        string status = "Active";

        UserCreateDTO user = new UserCreateDTO
                        {
                            Name = name, 
                            UserName = username, 
                            Password = password, 
                            Role = role, 
                            Status = status
                        };
        UserDTO createdUser = new UserDTO
                        {
                            Id = 1,
                            Name = name,
                            UserName = username,
                            Role = role,
                            Status = status
                        };
        _mockUserService.Setup(s => s.IsRoleValid(user.Role))
                        .Returns(true);
        _mockUserService.Setup(s => s.StatusMappingString2Enum(user.Status))
                        .Returns(UserStatus.Active);
        _mockUserService.Setup(s => s.IsStatusValid(UserStatus.Active))
                        .Returns(true);
        _mockUserService.Setup(s => s.CreateUser(user))
                        .Returns(createdUser);

        var result = _userController.CreateUser(user);

        Assert.IsType<CreatedAtActionResult>(result.Result); 
        Assert.Equal(createdUser, (result.Result as CreatedAtActionResult).Value);
    }      

    [Fact]
    public void CreateUser_NullUserCreateDTO_ReturnBadRequest()

    {
        var expected = "The UserCreateDTO is null.";
        var result = _userController.CreateUser(null);
        var badRequstResult = result.Result as BadRequestObjectResult;
        Assert.IsType<BadRequestObjectResult>(badRequstResult);
        Assert.Equal(expected, badRequstResult.Value);
    }

    [Fact]
    public void CreateUser_InvalidRole_ReturnBadRequest()

    {
        string name = "May Nicolaos";
        string username = "MayNicolaos";
        string password = "M@yNic01@0s";
        string role = "Admi";
        string status = "Active";

        UserCreateDTO user = new UserCreateDTO
                        {
                            Name = name, 
                            UserName = username, 
                            Password = password, 
                            Role = role, 
                            Status = status
                        };
        var expected = "Role can be User or Admin only.";
        _mockUserService.Setup(s => s.IsRoleValid(role))
                        .Returns(false);
        var result = _userController.CreateUser(user);
        var badRequstResult = result.Result as BadRequestObjectResult;
        Assert.IsType<BadRequestObjectResult>(badRequstResult);
        Assert.Equal(expected, badRequstResult.Value);
    }

    [Fact]
    public void CreateUser_NullRole_ReturnBadRequest()

    {
        string name = "May Nicolaos";
        string username = "MayNicolaos";
        string password = "M@yNic01@0s";
        string role = null;
        string status = "Active";

        UserCreateDTO user = new UserCreateDTO
                        {
                            Name = name, 
                            UserName = username, 
                            Password = password, 
                            Role = role, 
                            Status = status
                        };
        var expected = "Please provide user role.";
        _mockUserService.Setup(s => s.IsRoleValid(role))
                        .Returns(false);
        var result = _userController.CreateUser(user);
        var badRequstResult = result.Result as BadRequestObjectResult;
        Assert.IsType<BadRequestObjectResult>(badRequstResult);
        Assert.Equal(expected, badRequstResult.Value);
    }   

    [Fact]
    public void CreateUser_MissingRole_ReturnBadRequest()

    {
        string name = "May Nicolaos";
        string username = "MayNicolaos";
        string password = "M@yNic01@0s";
        string role = "";
        string status = "Active";

        UserCreateDTO user = new UserCreateDTO
                        {
                            Name = name, 
                            UserName = username, 
                            Password = password, 
                            Role = role, 
                            Status = status
                        };
        var expected = "Please provide user role.";
        _mockUserService.Setup(s => s.IsRoleValid(role))
                        .Returns(false);
        var result = _userController.CreateUser(user);
        var badRequstResult = result.Result as BadRequestObjectResult;
        Assert.IsType<BadRequestObjectResult>(badRequstResult);
        Assert.Equal(expected, badRequstResult.Value);
    }          

    [Fact]
    public void CreateUser_InvalidStatus_ReturnBadRequest()

    {
        string name = "May Nicolaos";
        string username = "MayNicolaos";
        string password = "M@yNic01@0s";
        string role = "Admin";
        string status = "Activ";

        UserCreateDTO user = new UserCreateDTO
                        {
                            Name = name, 
                            UserName = username, 
                            Password = password, 
                            Role = role, 
                            Status = status
                        };
        var expected = "Status can be Active or Inactive only.";
        _mockUserService.Setup(s => s.IsRoleValid(role))
                        .Returns(true);
        _mockUserService.Setup(s => s.IsStatusValid(It.IsAny<UserStatus>()))
                        .Returns(false);
        var result = _userController.CreateUser(user);
        var badRequstResult = result.Result as BadRequestObjectResult;
        Assert.IsType<BadRequestObjectResult>(badRequstResult);
        Assert.Equal(expected, badRequstResult.Value);
    }

    [Fact]
    public void CreateUser_NullStatus_ReturnBadRequest()

    {
        string name = "May Nicolaos";
        string username = "MayNicolaos";
        string password = "M@yNic01@0s";
        string role = "Admin";
        string status = null;

        UserCreateDTO user = new UserCreateDTO
                        {
                            Name = name, 
                            UserName = username, 
                            Password = password, 
                            Role = role, 
                            Status = status
                        };
        var expected = "Please provide user status.";
        _mockUserService.Setup(s => s.IsRoleValid(role))
                        .Returns(true);
        _mockUserService.Setup(s => s.IsStatusValid(It.IsAny<UserStatus>()))
                        .Returns(false);
        var result = _userController.CreateUser(user);
        var badRequstResult = result.Result as BadRequestObjectResult;
        Assert.IsType<BadRequestObjectResult>(badRequstResult);
        Assert.Equal(expected, badRequstResult.Value);
    }

    [Fact]
    public void CreateUser_MissingStatus_ReturnBadRequest()

    {
        string name = "May Nicolaos";
        string username = "MayNicolaos";
        string password = "M@yNic01@0s";
        string role = "Admin";
        string status = "";

        UserCreateDTO user = new UserCreateDTO
                        {
                            Name = name, 
                            UserName = username, 
                            Password = password, 
                            Role = role, 
                            Status = status
                        };
        var expected = "Please provide user status.";

        _mockUserService.Setup(s => s.IsRoleValid(role))
                        .Returns(true);
        var result = _userController.CreateUser(user);
        var badRequstResult = result.Result as BadRequestObjectResult;
        Assert.IsType<BadRequestObjectResult>(badRequstResult);
        Assert.Equal(expected, badRequstResult.Value);
    }           

    [Fact]
    public void CreateUser_UserCreationFailed_ReturnBadRequest()

    {
        string name = "May Nicolaos";
        string username = "MayNicolaos";
        string password = "M@yNic01@0s";
        string role = "Admin";
        string status = "Active";

        UserCreateDTO user = new UserCreateDTO
                        {
                            Name = name, 
                            UserName = username, 
                            Password = password, 
                            Role = role, 
                            Status = status
                        };
        var expected = "User is not created successfully. Username is in use. Please choose another one.";

        _mockUserService.Setup(s => s.IsRoleValid(role))
                        .Returns(true);
        _mockUserService.Setup(s => s.IsStatusValid(UserStatus.Active))
                        .Returns(true);
        _mockUserService.Setup(s => s.CreateUser(user))
                        .Returns((UserDTO) null);
        var result = _userController.CreateUser(user);
        var badRequstResult = result.Result as BadRequestObjectResult;
        Assert.IsType<BadRequestObjectResult>(badRequstResult);
        Assert.Equal(expected, badRequstResult.Value);
    }  
}