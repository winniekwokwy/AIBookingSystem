using System.Net.Http.Json;  
using AIBookingSystem.DTOs;  
namespace AIBookingSystem.IntegrationTests
{
    // Integration test class for testing the Orders API endpoints.
    // Uses the CustomWebApplicationFactory to create a test server and HttpClient for in-memory HTTP requests.
    public class RoomBookingIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;  // HttpClient instance used to send HTTP requests to the in-memory API
        // Constructor receives the factory instance from xUnit via IClassFixture
        // Factory creates the in-memory test server and HttpClient
        public OrderIntegrationTests(CustomWebApplicationFactory factory)
        {
            // Creates an HttpClient configured to communicate with the test server
            _client = factory.CreateClient(); 
        }
        // Test case to verify that creating an order with valid data returns success and expected response
        [Fact]
        public async Task CreateOrder_ValidRequest_ReturnsSuccess()
        {
            // Arrange: Prepare a valid OrderCreateDTO with a known CustomerId and a list of order items
            var orderDto = new OrderCreateDTO
            {
                CustomerId = 1,  // Assumes this customer exists in seeded test data
                Items = new List<OrderItemDTO>
                {
                    new OrderItemDTO { ProductId = 1, Quantity = 2 }  // Order 2 units of ProductId 1
                }
            };
            // Act: Send a POST request to the /api/orders endpoint with the orderDto serialized as JSON
            var response = await _client.PostAsJsonAsync("/api/orders", orderDto);
            // Assert: Verify the response indicates a successful HTTP status code (200-299)
            response.EnsureSuccessStatusCode();
            // Deserialize the JSON response content into an OrderResponseDTO object
            var result = await response.Content.ReadFromJsonAsync<OrderResponseDTO>();
            // Assert: Validate the response DTO is not null and contains the expected CustomerId
            Assert.NotNull(result);
            Assert.Equal(orderDto.CustomerId, result.CustomerId);
        }
        // Test case to verify fetching an existing order returns the expected order details
        [Fact]
        public async Task GetOrder_ExistingOrder_ReturnsSuccess()
        {
            // Arrange: First, create an order explicitly to ensure there is an order to fetch later
            var orderDto = new OrderCreateDTO
            {
                CustomerId = 1,  // Use the seeded test customer
                Items = new List<OrderItemDTO>
                {
                    new OrderItemDTO { ProductId = 1, Quantity = 2 }
                }
            };
            // Send POST request to create the order and ensure success
            var createResponse = await _client.PostAsJsonAsync("/api/orders", orderDto);
            createResponse.EnsureSuccessStatusCode();
            // Deserialize the created order response to get the assigned OrderId
            var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderResponseDTO>();
            var orderId = createdOrder?.OrderId;  // Safely extract OrderId for the next request
            // Act: Send a GET request to fetch the order by its OrderId
            var response = await _client.GetAsync($"/api/orders/{orderId}");
            // Assert: Confirm the GET request succeeded
            response.EnsureSuccessStatusCode();
            // Deserialize the order details from the response
            var result = await response.Content.ReadFromJsonAsync<OrderResponseDTO>();
            // Assert: Validate the response is not null and the returned OrderId matches the requested one
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
        }
    }
}