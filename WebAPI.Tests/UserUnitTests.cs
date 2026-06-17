// using AutoFixture;

// namespace WebAPI.Tests;

// public class UserUnitTests: IClassFixture<WebApplicationFactory<Api.Startup>>
// {
//     readonly HttpClient _client;

//     public UserUnitTests(WebApplicationFactory<Api.Startup> application)
//     {
//         _client = application.CreateClient();
//     }

//     [Fact]
//     public void ListUsers()
//     {
//         var response = await _client.GetAsync("/api/UserController/ListUsers");
//         response.StatusCode.Should().Be(HttpStatusCode.OK);
//     }
// }
