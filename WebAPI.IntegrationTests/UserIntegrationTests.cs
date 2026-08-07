using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata;
using System.Net;
using Moq;
using System.Net.Http.Headers;

using AIBookingSystem.DTO;  
using AIBookingSystem.Controllers;
using AIBookingSystem.Services;
using System.ComponentModel.DataAnnotations;

namespace AIBookingSystem.IntegrationTests
{
    // Integration test class for testing the Orders API endpoints.
    // Uses the CustomWebApplicationFactory to create a test server and HttpClient for in-memory HTTP requests.
    public class UserIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;  // HttpClient instance used to send HTTP requests to the in-memory API
        private readonly ITestOutputHelper _output;
        private readonly InMemoryLoggerProvider _logger;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        // Constructor receives the factory instance from xUnit via IClassFixture
        // Factory creates the in-memory test server and HttpClient
        public UserIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
        {
            // Creates an HttpClient configured to communicate with the test server
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://localhost:7287") // Adjust the port as needed
            }); 
            _output = output;
            _logger = factory.LoggerProvider;
        }

        [Fact]
        public async Task CreateUser_AdminUser_ReturnOK()
        {
            string name = "May Nicolaos";
            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            string role = "Admin";
            string status = "Active";
            var clientId = "client-app-one";

            var userLoginRequest = new UserLoginDTO
            {
                UserName = "applemango",
                Password = "App13M@ng0",
                ClientId = clientId
            };

            var jsonContent4Login = JsonSerializer.Serialize(userLoginRequest);
            var content4Login = new StringContent(jsonContent4Login, Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/api/Auth/login", content4Login, TestContext.Current.CancellationToken);
        
            // Ensure the login succeeded before attempting to deserialize JSON
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            _output.WriteLine($"LoginResponseBody: {loginResponseBody}");
            foreach (var entry in _logger.Entries)
            {
                _output.WriteLine($"{entry.Level} | {entry.Category} | {entry.Message}");
            }
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromLogin);

            // _output.WriteLine($"refreshToken : {loginAuthResponse.RefreshToken}");

            UserCreateDTO user = new UserCreateDTO
                            {
                                Name = name, 
                                UserName = username, 
                                Password = password, 
                                Role = role, 
                                Status = status
                            };

            var jsonContent4CreateUser = JsonSerializer.Serialize(user);
            var content4CreateUser = new StringContent(jsonContent4CreateUser, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var createUserResponse = await _client.PostAsync("/api/User/create-user", content4CreateUser, TestContext.Current.CancellationToken);

            var createUserResponseBody = await createUserResponse.Content.ReadAsStringAsync(
               TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);  
            var userDTO = JsonSerializer.Deserialize<UserDTO>(createUserResponseBody, _jsonOptions);
            Assert.NotNull(userDTO);

            // // _output.WriteLine($"StatusCode: {(int)response.StatusCode} ({response.StatusCode})");
            // // _output.WriteLine($"ReasonPhrase: {response.ReasonPhrase}");
            // // _output.WriteLine($"ResponseBody: {responseBody}");
            // // _output.WriteLine($"RequestMessage: {response.RequestMessage}");

            // foreach (var entry in _logger.Entries)
            // {
            //     _output.WriteLine(
            //         $"{entry.Level} | {entry.Category} | {entry.Message}");
            // }

            Assert.Equal(username, userDTO.UserName);
            Assert.Equal(status, userDTO.Status);
        }

        [Fact]
        public async Task CreateUser_User_ReturnForbidden()
        {
            string name = "May Nicolaos 1";
            string username = "maynicolaos1";
            string password = "M@yNic01@0s";
            string role = "Admin";
            string status = "Active";
            var clientId = "client-app-one";

            var userLoginRequest = new UserLoginDTO
            {
                UserName = "bensmith",
                Password = "App13M@ng0",
                ClientId = clientId
            };

            var jsonContent4Login = JsonSerializer.Serialize(userLoginRequest);
            var content4Login = new StringContent(jsonContent4Login, Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/api/Auth/login", content4Login, TestContext.Current.CancellationToken);
        
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            _output.WriteLine($"LoginResponseBody: {loginResponseBody}");
            foreach (var entry in _logger.Entries)
            {
                _output.WriteLine($"{entry.Level} | {entry.Category} | {entry.Message}");
            }
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromLogin);

            _output.WriteLine($"refreshToken : {authResponseDTOFromLogin.RefreshToken}");

            UserCreateDTO user = new UserCreateDTO
                            {
                                Name = name, 
                                UserName = username, 
                                Password = password, 
                                Role = role, 
                                Status = status
                            };

            var jsonContent4CreateUser = JsonSerializer.Serialize(user);
            var content4CreateUser = new StringContent(jsonContent4CreateUser, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var createUserResponse = await _client.PostAsync("/api/User/create-user", content4CreateUser, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Forbidden, createUserResponse.StatusCode);   

            _output.WriteLine($"StatusCode: {(int)createUserResponse.StatusCode} ({createUserResponse.StatusCode})");
            _output.WriteLine($"ReasonPhrase: {createUserResponse.ReasonPhrase}");
            _output.WriteLine($"RequestMessage: {createUserResponse.RequestMessage}");

            foreach (var entry in _logger.Entries)
            {
                _output.WriteLine(
                    $"{entry.Level} | {entry.Category} | {entry.Message}");
            }
        }

        [Fact]
        public async Task CreateUser_NoToken_ReturnsUnauthorized()
        {
            var user = new UserCreateDTO
            {
                Name = "No Token",
                UserName = "notokenuser",
                Password = "P@ssw0rd!",
                Role = "Admin",
                Status = "Active"
            };

            var jsonContent = JsonSerializer.Serialize(user);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Ensure no Authorization header is sent
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.PostAsync("/api/User/create-user", content, TestContext.Current.CancellationToken);

            _output.WriteLine($"StatusCode: {(int)response.StatusCode} ({response.StatusCode})");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateUser_OldToken_ReturnUnauthorized()
        {
            string name = "May Nicolaos 2";
            string username = "maynicolaos2";
            string password = "M@yNic01@0s";
            string role = "Admin";
            string status = "Active";
            var clientId = "client-app-one";

            var userLoginRequest = new UserLoginDTO
            {
                UserName = "applemango",
                Password = "App13M@ng0",
                ClientId = clientId
            };

            var jsonContent4Login = JsonSerializer.Serialize(userLoginRequest);
            var content4Login = new StringContent(jsonContent4Login, Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/api/Auth/login", content4Login, TestContext.Current.CancellationToken);
        
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromLogin);
            // _output.WriteLine($"refreshToken : {loginAuthResponse.RefreshToken}");

            var refreshTokenDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = authResponseDTOFromLogin.RefreshToken,
                ClientId = clientId
            };

            var jsonContent4RefreshToken = JsonSerializer.Serialize(refreshTokenDTO);
            var content4RefreshToken = new StringContent(jsonContent4RefreshToken, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var refreshTokenResponse = await _client.PostAsync("/api/Auth/refresh-token", content4RefreshToken, TestContext.Current.CancellationToken);

            var refreshTokenResponseBody = await refreshTokenResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, refreshTokenResponse.StatusCode);
            var authResponseDTO4FromRefreshToken = JsonSerializer.Deserialize<AuthResponseDTO>(refreshTokenResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTO4FromRefreshToken);
            // var refreshTokenResponse = await response2.Content.ReadFromJsonAsync<AuthResponseDTO>(TestContext.Current.CancellationToken);
            // Assert.NotNull(refreshTokenResponse);
            Assert.NotEqual(authResponseDTOFromLogin.AccessToken, authResponseDTO4FromRefreshToken.AccessToken);

            UserCreateDTO user = new UserCreateDTO
                            {
                                Name = name, 
                                UserName = username, 
                                Password = password, 
                                Role = role, 
                                Status = status
                            };

            var jsonContent4CreateUser = JsonSerializer.Serialize(user);
            var content4CreateUser = new StringContent(jsonContent4CreateUser, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var createUserResponse = await _client.PostAsync("/api/User/create-user", content4CreateUser, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Unauthorized, createUserResponse.StatusCode);   

            // _output.WriteLine($"StatusCode: {(int)response.StatusCode} ({response.StatusCode})");
            // _output.WriteLine($"ReasonPhrase: {response.ReasonPhrase}");
            // _output.WriteLine($"ResponseBody: {responseBody}");
            // _output.WriteLine($"RequestMessage: {response.RequestMessage}");

            // foreach (var entry in _logger.Entries)
            // {
            //     _output.WriteLine(
            //         $"{entry.Level} | {entry.Category} | {entry.Message}");
            // }
        }

        [Fact]
        public async Task ListUsers_Admin_ReturnUserDTOs()
        {
            var clientId = "client-app-one";

            var userLoginRequest = new UserLoginDTO
            {
                UserName = "applemango",
                Password = "App13M@ng0",
                ClientId = clientId
            };

            var jsonContent4Login = JsonSerializer.Serialize(userLoginRequest);
            var content4Login = new StringContent(jsonContent4Login, Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/api/Auth/login", content4Login, TestContext.Current.CancellationToken);
        
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromLogin);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var listUsersResponse = await _client.GetAsync("api/User/users", TestContext.Current.CancellationToken);

            var listUsersResponseBody = await listUsersResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, listUsersResponse.StatusCode);
            var userDTOList = JsonSerializer.Deserialize<IEnumerable<UserDTO>>(listUsersResponseBody, _jsonOptions);
            Assert.NotNull(userDTOList);
            Assert.Equal(2, userDTOList.Count());
        }

        [Fact]
        public async Task ListUsers_User_ReturnForbidden()
        {
            var clientId = "client-app-one";

            var userLoginRequest = new UserLoginDTO
            {
                UserName = "bensmith",
                Password = "App13M@ng0",
                ClientId = clientId
            };

            var jsonContent4Login = JsonSerializer.Serialize(userLoginRequest);
            var content4Login = new StringContent(jsonContent4Login, Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/api/Auth/login", content4Login, TestContext.Current.CancellationToken);
        
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromLogin);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var listUsersResponse = await _client.GetAsync("api/User/users", TestContext.Current.CancellationToken);

            var listUsersResponseBody = await listUsersResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Forbidden, listUsersResponse.StatusCode);
        }

        [Fact]
        public async Task ListUsers_OldToken_ReturnUnauthorized()
        {
            var clientId = "client-app-one";

            var userLoginRequest = new UserLoginDTO
            {
                UserName = "bensmith",
                Password = "App13M@ng0",
                ClientId = clientId
            };

            var jsonContent4Login = JsonSerializer.Serialize(userLoginRequest);
            var content4Login = new StringContent(jsonContent4Login, Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/api/Auth/login", content4Login, TestContext.Current.CancellationToken);
        
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromLogin);

            var refreshTokenRequestDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = authResponseDTOFromLogin.RefreshToken,
                ClientId = clientId
            };

            var jsonContent4RefreshToken = JsonSerializer.Serialize(refreshTokenRequestDTO);
            var content4RefreshToken = new StringContent(jsonContent4RefreshToken, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var refreshTokenResponse = await _client.PostAsync("/api/Auth/refresh-token", content4RefreshToken, TestContext.Current.CancellationToken);
        
            var refreshTokenResponseBody = await refreshTokenResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, refreshTokenResponse.StatusCode);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var listUsersResponse = await _client.GetAsync("api/User/users", TestContext.Current.CancellationToken);

            var listUsersResponseBody = await listUsersResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Unauthorized, listUsersResponse.StatusCode);
        }

        [Fact]
        public async Task ListUsers_NoToken_ReturnsUnauthorized()
        {
            // Ensure no Authorization header is sent
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync("/api/User/users", TestContext.Current.CancellationToken);

            _output.WriteLine($"StatusCode: {(int)response.StatusCode} ({response.StatusCode})");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]

        public async Task GetUserbyId_Admin_ReturnUser()
        {
            var clientId = "client-app-one";

            var userLoginRequest = new UserLoginDTO
            {
                UserName = "applemango",
                Password = "App13M@ng0",
                ClientId = clientId
            };

            var jsonContent4Login = JsonSerializer.Serialize(userLoginRequest);
            var content4Login = new StringContent(jsonContent4Login, Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/api/Auth/login", content4Login, TestContext.Current.CancellationToken);
        
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromLogin);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var usersResponse = await _client.GetAsync("api/User/get-user-by-id/2/", TestContext.Current.CancellationToken);

            var usersResponseBody = await usersResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, usersResponse.StatusCode);
            var userDTO = JsonSerializer.Deserialize<UserDTO>(usersResponseBody, _jsonOptions);
            Assert.NotNull(userDTO);
            Assert.Equal("bensmith", userDTO.UserName);
        }

        [Fact]
        public async Task GetUserbyId_User_ReturnUser()
        {
            var clientId = "client-app-one";

            var userLoginRequest = new UserLoginDTO
            {
                UserName = "bensmith",
                Password = "App13M@ng0",
                ClientId = clientId
            };

            var jsonContent4Login = JsonSerializer.Serialize(userLoginRequest);
            var content4Login = new StringContent(jsonContent4Login, Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/api/Auth/login", content4Login, TestContext.Current.CancellationToken);
        
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromLogin);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var usersResponse = await _client.GetAsync("api/User/get-user-by-id/1/", TestContext.Current.CancellationToken);

            var usersResponseBody = await usersResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, usersResponse.StatusCode);
            var userDTO = JsonSerializer.Deserialize<UserDTO>(usersResponseBody, _jsonOptions);
            Assert.NotNull(userDTO);
            Assert.Equal("applemango", userDTO.UserName);
        }

        [Fact]
        public async Task GetUserbyId_OldToken_ReturnUnauthorized()
        {
            var clientId = "client-app-one";

            var userLoginRequest = new UserLoginDTO
            {
                UserName = "applemango",
                Password = "App13M@ng0",
                ClientId = clientId
            };

            var jsonContent4Login = JsonSerializer.Serialize(userLoginRequest);
            var content4Login = new StringContent(jsonContent4Login, Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/api/Auth/login", content4Login, TestContext.Current.CancellationToken);
        
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromLogin);

            var refreshTokenRequestDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = authResponseDTOFromLogin.RefreshToken,
                ClientId = clientId
            };

            var jsonContent4RefreshToken = JsonSerializer.Serialize(refreshTokenRequestDTO);
            var content4RefreshToken = new StringContent(jsonContent4RefreshToken, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var refreshTokenResponse = await _client.PostAsync("/api/Auth/refresh-token", content4RefreshToken, TestContext.Current.CancellationToken);
        
            var refreshTokenResponseBody = await refreshTokenResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, refreshTokenResponse.StatusCode);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var usersResponse = await _client.GetAsync("api/User/get-user-by-id/2/", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Unauthorized, usersResponse.StatusCode);
        }

        [Fact]
        public async Task GetUserbyId_NoToken_ReturnUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var usersResponse = await _client.GetAsync("api/User/get-user-by-id/2/", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Unauthorized, usersResponse.StatusCode);
        }

               [Fact]

        public async Task GetUserbyUsername_Admin_ReturnUser()
        {
            var clientId = "client-app-one";
            var username = "bensmith";

            var userLoginRequest = new UserLoginDTO
            {
                UserName = "applemango",
                Password = "App13M@ng0",
                ClientId = clientId
            };

            var jsonContent4Login = JsonSerializer.Serialize(userLoginRequest);
            var content4Login = new StringContent(jsonContent4Login, Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/api/Auth/login", content4Login, TestContext.Current.CancellationToken);
        
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromLogin);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var usersResponse = await _client.GetAsync($"api/User/get-user-by-username/{username}/", TestContext.Current.CancellationToken);

            var usersResponseBody = await usersResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, usersResponse.StatusCode);
            var userDTO = JsonSerializer.Deserialize<UserDTO>(usersResponseBody, _jsonOptions);
            Assert.NotNull(userDTO);
            Assert.Equal(username, userDTO.UserName);
        }

        [Fact]
        public async Task GetUserbyUsername_User_ReturnUser()
        {
            var clientId = "client-app-one";
            var username = "applemango";

            var userLoginRequest = new UserLoginDTO
            {
                UserName = "bensmith",
                Password = "App13M@ng0",
                ClientId = clientId
            };

            var jsonContent4Login = JsonSerializer.Serialize(userLoginRequest);
            var content4Login = new StringContent(jsonContent4Login, Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/api/Auth/login", content4Login, TestContext.Current.CancellationToken);
        
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromLogin);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var usersResponse = await _client.GetAsync($"api/User/get-user-by-username/{username}/", TestContext.Current.CancellationToken);

            var usersResponseBody = await usersResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, usersResponse.StatusCode);
            var userDTO = JsonSerializer.Deserialize<UserDTO>(usersResponseBody, _jsonOptions);
            Assert.NotNull(userDTO);
            Assert.Equal(username, userDTO.UserName);
        }

        [Fact]
        public async Task GetUserbyUsername_OldToken_ReturnUnauthorized()
        {
            var clientId = "client-app-one";
            var username = "bensmith";

            var userLoginRequest = new UserLoginDTO
            {
                UserName = "applemango",
                Password = "App13M@ng0",
                ClientId = clientId
            };

            var jsonContent4Login = JsonSerializer.Serialize(userLoginRequest);
            var content4Login = new StringContent(jsonContent4Login, Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/api/Auth/login", content4Login, TestContext.Current.CancellationToken);
        
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromLogin);

            var refreshTokenRequestDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = authResponseDTOFromLogin.RefreshToken,
                ClientId = clientId
            };

            var jsonContent4RefreshToken = JsonSerializer.Serialize(refreshTokenRequestDTO);
            var content4RefreshToken = new StringContent(jsonContent4RefreshToken, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var refreshTokenResponse = await _client.PostAsync("/api/Auth/refresh-token", content4RefreshToken, TestContext.Current.CancellationToken);
        
            var refreshTokenResponseBody = await refreshTokenResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, refreshTokenResponse.StatusCode);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var usersResponse = await _client.GetAsync($"api/User/get-user-by-username/{username}/", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Unauthorized, usersResponse.StatusCode);
        }

        [Fact]
        public async Task GetUserbyUsername_NoToken_ReturnUnauthorized()
        {
            var username = "applemango";

            _client.DefaultRequestHeaders.Authorization = null;
            var usersResponse = await _client.GetAsync($"api/User/get-user-by-username/{username}/", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Unauthorized, usersResponse.StatusCode);
        }
    }
}