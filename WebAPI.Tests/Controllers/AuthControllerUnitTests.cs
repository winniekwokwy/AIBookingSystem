using AIBookingSystem.Controllers;
using AIBookingSystem.Services;
using AIBookingSystem.DTO;
using AIBookingSystem.Enums;

using Moq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace WebAPI.Tests.Controllers

{
    public class AuthControllerUnitTests
    {
        private readonly Mock<IUserService> _mockUserService;

        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly IConfiguration _configuration;

        public AuthControllerUnitTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _configuration = new ConfigurationBuilder()
                        .AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["JwtSettings:AccessTokenExpirationMinutes"] = "15"
                        })
                        .Build();
        }

        private AuthController CreateController(string remoteIp = "127.0.0.1")
        {
            var controller = new AuthController(_mockUserService.Object, _mockLogger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            controller.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse(remoteIp);
            return controller;
        }

        [Fact]
        public async Task Login_ValidUser_ReturnOK()
        {
            string username = "applemango";
            string password = "App13M@ng0";
            string clientId = "client-app-one";

            var loginDTO = new UserLoginDTO
            {
                UserName = username,
                Password = password,
                ClientId = clientId
            };

            string ipAddress = "127.0.0.1";
            var accessTokenExpiryMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;

            AuthResponseDTO authResponse = new AuthResponseDTO
            {
                AccessToken = "access Token",
                RefreshToken = "refresh Token",
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes)
            };

            _mockUserService.Setup(s => s.AuthenticateUser(loginDTO, ipAddress))
                            .ReturnsAsync(authResponse);

            var authController = CreateController();
            var result = await authController.Login(loginDTO);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<AuthResponseDTO>(okResult.Value);
            Assert.Equal(authResponse.AccessToken, returned.AccessToken);
            Assert.Equal(authResponse.RefreshToken, returned.RefreshToken);
            Assert.True(returned.AccessTokenExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public async Task Login_UnauthenticatedUser_ReturnUnauthorized()
        {
            string username = "applemango1";
            string password = "App13M@ng0";
            string clientId = "client-app-one";
            string expected = "Invalid credentials or client.";

            var loginDTO = new UserLoginDTO
            {
                UserName = username,
                Password = password,
                ClientId = clientId
            };

            string ipAddress = "127.0.0.1";

            _mockUserService.Setup(s => s.AuthenticateUser(loginDTO, ipAddress))
                            .ReturnsAsync((AuthResponseDTO?)null!);

            var authController = CreateController();
            var result = await authController.Login(loginDTO);

            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            Assert.IsType<UnauthorizedObjectResult>(unauthorizedResult);
            Assert.Equal(expected, unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_InvalidInput_ReturnBadRequest()
        {
            string password = "App13M@ng0";
            string clientId = "client-app-one";

            var loginDTO = new UserLoginDTO
            {
                UserName = null!,
                Password = password,
                ClientId = clientId
            };

            var authController = CreateController();
            authController.ModelState.AddModelError("username", "Username is required.");
            var result = await authController.Login(loginDTO);

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequestResult);
        }

        [Fact]
        public async Task Login_EmptyInput_ReturnBadRequest()
        {
            string password = "App13M@ng0";
            string clientId = "client-app-one";

            var loginDTO = new UserLoginDTO
            {
                UserName = "",
                Password = password,
                ClientId = clientId
            };

            var authController = CreateController();
            authController.ModelState.AddModelError("username", "Username must at least have 8 characters.");
            var result = await authController.Login(loginDTO);

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequestResult);
        }

        [Fact]
        public async Task RefreshToken_ValidRefreshTokenRequestDTO_ReturnOK()
        {
            string token = "refresh token";
            string clientId = "client-app-one";

            var refreshTokenRequestDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = token,
                ClientId = clientId
            };

            string ipAddress = "127.0.0.1";
            var accessTokenExpiryMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;

            AuthResponseDTO authResponse = new AuthResponseDTO
            {
                AccessToken = "access Token",
                RefreshToken = "refresh Token",
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes)
            };

            _mockUserService.Setup(s => s.RefreshToken(refreshTokenRequestDTO.RefreshToken, refreshTokenRequestDTO.ClientId, ipAddress))
                            .ReturnsAsync(authResponse);

            var authController = CreateController();
            var result = await authController.RefreshToken(refreshTokenRequestDTO);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<AuthResponseDTO>(okResult.Value);
            Assert.Equal(authResponse.AccessToken, returned.AccessToken);
            Assert.Equal(authResponse.RefreshToken, returned.RefreshToken);
            Assert.True(returned.AccessTokenExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public async Task RefreshToken_ServiceReturnFailed_ReturnUnauthorized()
        {
            string token = "refresh token";
            string clientId = "client-app-one";
            string expected = "Invalid refresh token or client.";

            var refreshTokenRequestDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = token,
                ClientId = clientId
            };

            string ipAddress = "127.0.0.1";
            var accessTokenExpiryMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;

            AuthResponseDTO authResponse = new AuthResponseDTO
            {
                AccessToken = "access Token",
                RefreshToken = "refresh Token",
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes)
            };

            _mockUserService.Setup(s => s.RefreshToken(refreshTokenRequestDTO.RefreshToken, refreshTokenRequestDTO.ClientId, ipAddress))
                            .ReturnsAsync((AuthResponseDTO?)null!);

            var authController = CreateController();
            var result = await authController.RefreshToken(refreshTokenRequestDTO);

            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            Assert.IsType<UnauthorizedObjectResult>(unauthorizedResult);
            Assert.Equal(expected, unauthorizedResult.Value);
        }

        [Fact]
        public async Task RefreshToken_InvalidInput_ReturnBadRequest()
        {
            string token = "refresh token";
            string clientId = "client-app-one";

            var refreshTokenRequestDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = token,
                ClientId = clientId
            };

            var authController = CreateController();
            authController.ModelState.AddModelError("RefreshToken", "RefreshToken is required.");
            var result = await authController.RefreshToken(refreshTokenRequestDTO);

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequestResult);
        }
    }
}