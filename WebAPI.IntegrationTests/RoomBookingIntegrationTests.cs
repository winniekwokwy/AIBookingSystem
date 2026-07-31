using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

using AIBookingSystem.DTO;  
using AIBookingSystem.Controllers;
using System.Reflection.Metadata;
using System.Net;
namespace AIBookingSystem.IntegrationTests
{
    // Integration test class for testing the Orders API endpoints.
    // Uses the CustomWebApplicationFactory to create a test server and HttpClient for in-memory HTTP requests.
    public class RoomBookingIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;  // HttpClient instance used to send HTTP requests to the in-memory API
        private readonly ITestOutputHelper _output;

        private readonly InMemoryLoggerProvider _logger;
        // Constructor receives the factory instance from xUnit via IClassFixture
        // Factory creates the in-memory test server and HttpClient
        public RoomBookingIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
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

            var responseBody = await response.Content.ReadAsStringAsync(
                TestContext.Current.CancellationToken);

            _output.WriteLine($"StatusCode: {(int)response.StatusCode} ({response.StatusCode})");
            _output.WriteLine($"ReasonPhrase: {response.ReasonPhrase}");
            _output.WriteLine($"ResponseBody: {responseBody}");
            _output.WriteLine($"RequestMessage: {response.RequestMessage}");

            foreach (var entry in _logger.Entries)
            {
                _output.WriteLine(
                    $"{entry.Level} | {entry.Category} | {entry.Message}");
            }

            Assert.True(
                response.IsSuccessStatusCode,
                $"Login failed with {(int)response.StatusCode}: {responseBody}");
                
            var userLoginResponse = await response.Content.ReadFromJsonAsync<AuthResponseDTO>(TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);                
        }

        [Fact]
        public async Task Login_InvalidLoginCredential_ReturnUnauthorized()
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

            _output.WriteLine($"StatusCode: {(int)response.StatusCode} ({response.StatusCode})");
            _output.WriteLine($"ReasonPhrase: {response.ReasonPhrase}");
            _output.WriteLine($"ResponseBody: {responseBody}");
            _output.WriteLine($"RequestMessage: {response.RequestMessage}");

            foreach (var entry in _logger.Entries)
            {
                _output.WriteLine(
                    $"{entry.Level} | {entry.Category} | {entry.Message}");
            }

            Assert.False(
                response.IsSuccessStatusCode,
                $"Login failed with {(int)response.StatusCode}: {responseBody}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

                [Fact]
        public async Task Login_InvalidInput_ReturnBadRequest()
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

            _output.WriteLine($"StatusCode: {(int)response.StatusCode} ({response.StatusCode})");
            _output.WriteLine($"ReasonPhrase: {response.ReasonPhrase}");
            _output.WriteLine($"ResponseBody: {responseBody}");
            _output.WriteLine($"RequestMessage: {response.RequestMessage}");

            foreach (var entry in _logger.Entries)
            {
                _output.WriteLine(
                    $"{entry.Level} | {entry.Category} | {entry.Message}");
            }

            Assert.False(
                response.IsSuccessStatusCode,
                $"Login failed with {(int)response.StatusCode}: {responseBody}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}