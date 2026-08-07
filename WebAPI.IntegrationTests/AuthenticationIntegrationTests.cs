using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata;
using System.Net;
using Moq;

using AIBookingSystem.DTO;  
using AIBookingSystem.Controllers;
using AIBookingSystem.Services;
using System.Net.Http.Headers;

namespace AIBookingSystem.IntegrationTests
{
    // Integration test class for testing the Orders API endpoints.
    // Uses the CustomWebApplicationFactory to create a test server and HttpClient for in-memory HTTP requests.
    public class AuthenticationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;  // HttpClient instance used to send HTTP requests to the in-memory API
        private readonly ITestOutputHelper _output;
        private readonly InMemoryLoggerProvider _logger;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        // Constructor receives the factory instance from xUnit via IClassFixture
        // Factory creates the in-memory test server and HttpClient
        public AuthenticationIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
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
        public async Task Login_ValidLoginCredential_ReturnOK()
        {
            var userLoginRequest = new UserLoginDTO
            {
                UserName = "applemango",
                Password = "App13M@ng0",
                ClientId = "client-app-one"
            };

            var jsonContent = JsonSerializer.Serialize(userLoginRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/Auth/login", content, TestContext.Current.CancellationToken);

            var responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var authResponseDTO = JsonSerializer.Deserialize<AuthResponseDTO>(responseBody, _jsonOptions);
            Assert.NotNull(authResponseDTO);

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
        public async Task Login_IncorrectPassword_ReturnUnauthorized()
        {
            var userLoginRequest = new UserLoginDTO
            {
                UserName = "applemango",
                Password = "App13M@ng0!",
                ClientId = "client-app-one"
            };

            var jsonContent = JsonSerializer.Serialize(userLoginRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/Auth/login", content, TestContext.Current.CancellationToken);

            var responseBody = await response.Content.ReadAsStringAsync(
                TestContext.Current.CancellationToken);

            // _output.WriteLine($"StatusCode: {(int)response.StatusCode} ({response.StatusCode})");
            // _output.WriteLine($"ReasonPhrase: {response.ReasonPhrase}");
            // _output.WriteLine($"ResponseBody: {responseBody}");
            // _output.WriteLine($"RequestMessage: {response.RequestMessage}");

            // foreach (var entry in _logger.Entries)
            // {
            //     _output.WriteLine(
            //         $"{entry.Level} | {entry.Category} | {entry.Message}");
            // }

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_NonexistingUser_ReturnUnauthorized()
        {
            var userLoginRequest = new UserLoginDTO
            {
                UserName = "applemango1",
                Password = "App13M@ng0",
                ClientId = "client-app-one"
            };

            var jsonContent = JsonSerializer.Serialize(userLoginRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/Auth/login", content, TestContext.Current.CancellationToken);

            var responseBody = await response.Content.ReadAsStringAsync(
                TestContext.Current.CancellationToken);

            // _output.WriteLine($"StatusCode: {(int)response.StatusCode} ({response.StatusCode})");
            // _output.WriteLine($"ReasonPhrase: {response.ReasonPhrase}");
            // _output.WriteLine($"ResponseBody: {responseBody}");
            // _output.WriteLine($"RequestMessage: {response.RequestMessage}");

            // foreach (var entry in _logger.Entries)
            // {
            //     _output.WriteLine(
            //         $"{entry.Level} | {entry.Category} | {entry.Message}");
            // }

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_ClientIdNull_ReturnBadRequest()
        {
            var userLoginRequest = new UserLoginDTO
            {
                UserName = "applemango",
                Password = "App13M@ng0!",
                ClientId = null!
            };

            var jsonContent = JsonSerializer.Serialize(userLoginRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/Auth/login", content, TestContext.Current.CancellationToken);

            var responseBody = await response.Content.ReadAsStringAsync(
                TestContext.Current.CancellationToken);

            // _output.WriteLine($"StatusCode: {(int)response.StatusCode} ({response.StatusCode})");
            // _output.WriteLine($"ReasonPhrase: {response.ReasonPhrase}");
            // _output.WriteLine($"ResponseBody: {responseBody}");
            // _output.WriteLine($"RequestMessage: {response.RequestMessage}");

            // foreach (var entry in _logger.Entries)
            // {
            //     _output.WriteLine(
            //         $"{entry.Level} | {entry.Category} | {entry.Message}");
            // }

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_ValidInput_ReturnAuthResponse()
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
            var authResponseDTOFromRefreshToken = JsonSerializer.Deserialize<AuthResponseDTO>(refreshTokenResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromRefreshToken);

            Assert.NotEqual(authResponseDTOFromLogin.RefreshToken, authResponseDTOFromRefreshToken.RefreshToken);
            Assert.NotEqual(authResponseDTOFromLogin.AccessToken, authResponseDTOFromRefreshToken.AccessToken);
            Assert.True(authResponseDTOFromRefreshToken.AccessTokenExpiresAt > authResponseDTOFromLogin.AccessTokenExpiresAt);

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
        public async Task RefreshToken_OldToken_ReturnUnauthorized()
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
        
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(
                TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);            
            Assert.NotNull(authResponseDTOFromLogin);

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
            var authResponseDTOFromRefreshToken = JsonSerializer.Deserialize<AuthResponseDTO>(refreshTokenResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromRefreshToken); 
            Assert.NotEqual(authResponseDTOFromLogin.RefreshToken, authResponseDTOFromRefreshToken.RefreshToken);
            Assert.NotEqual(authResponseDTOFromLogin.AccessToken, authResponseDTOFromRefreshToken.AccessToken);
            Assert.True(authResponseDTOFromRefreshToken.AccessTokenExpiresAt > authResponseDTOFromLogin.AccessTokenExpiresAt);

            // _output.WriteLine($"StatusCode: {(int)response.StatusCode} ({response.StatusCode})");
            // _output.WriteLine($"ReasonPhrase: {response.ReasonPhrase}");
            // _output.WriteLine($"ResponseBody: {responseBody}");
            // _output.WriteLine($"RequestMessage: {response.RequestMessage}");

            // foreach (var entry in _logger.Entries)
            // {
            //     _output.WriteLine(
            //         $"{entry.Level} | {entry.Category} | {entry.Message}");
            // }

            var refreshTokenResponse2ndTime = await _client.PostAsync("/api/Auth/refresh-token", content4RefreshToken, TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Unauthorized, refreshTokenResponse2ndTime.StatusCode);  
        }

        [Fact]
        public async Task RefreshToken_InputNull_ReturnBadRequest()
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
        
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(
                TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromLogin);

            // _output.WriteLine($"refreshToken : {loginAuthResponse.RefreshToken}");

            var refreshTokenDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = authResponseDTOFromLogin.RefreshToken,
                ClientId = null!
            };

            var jsonContent4RefreshToken = JsonSerializer.Serialize(refreshTokenDTO);
            var content4RefreshToken = new StringContent(jsonContent4RefreshToken, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var refreshTokenResponse = await _client.PostAsync("/api/Auth/refresh-token", content4RefreshToken, TestContext.Current.CancellationToken);

            var refreshTokenResponseBody = refreshTokenResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.BadRequest, refreshTokenResponse.StatusCode);    

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
        public async Task RefreshToken_InputEmpty_ReturnBadRequest()
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
        
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync(
                TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);     
            var authResponseDTOFromLogin = JsonSerializer.Deserialize<AuthResponseDTO>(loginResponseBody, _jsonOptions);
            Assert.NotNull(authResponseDTOFromLogin);            

            // _output.WriteLine($"refreshToken : {authResponseDTOFromLogin.RefreshToken}");

            var refreshTokenDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = authResponseDTOFromLogin.RefreshToken,
                ClientId = ""
            };

            var jsonContent4RefreshToken = JsonSerializer.Serialize(refreshTokenDTO);
            var content4RefreshToken = new StringContent(jsonContent4RefreshToken, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponseDTOFromLogin.AccessToken);
            var refreshTokenResponse = await _client.PostAsync("/api/Auth/refresh-token", content4RefreshToken, TestContext.Current.CancellationToken);

            var refreshTokenResponseBody = await refreshTokenResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

            // _output.WriteLine($"StatusCode: {(int)refreshTokenResponse.StatusCode} ({refreshTokenResponse.StatusCode})");
            // _output.WriteLine($"ReasonPhrase: {refreshTokenResponse.ReasonPhrase}");
            // _output.WriteLine($"ResponseBody: {refreshTokenResponseBody}");
            // _output.WriteLine($"RequestMessage: {refreshTokenResponse.RequestMessage}");

            // foreach (var entry in _logger.Entries)
            // {
            //     _output.WriteLine(
            //         $"{entry.Level} | {entry.Category} | {entry.Message}");
            // }

            Assert.Equal(HttpStatusCode.BadRequest, refreshTokenResponse.StatusCode);   

        }
   }
}