using WebAPI.Controllers;
// using FluentAssertions;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Xunit;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockService;
    private readonly Mock<ILogger<UserController>> _mockLogger;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockService = new Mock<IUserService>();
        _mockLogger = new Mock<ILogger<UserController>>();
        _controller = new UserController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
public async Task GetUserById_ReturnsOk_WhenUserExists()
{
    // Arrange
    var product = new Product
    {
        Id = 1,
        Name = "Laptop",
        SKU = "LAP-001",
        Price = 999.99m,
        IsActive = true,
        Category = new Category { Id = 1, Name = "Electronics" }
    };

    _mockService.Setup(s => s.GetProductAsync(1))
        .ReturnsAsync(product);

    // Act
    var result = await _controller.GetById(1);

    // Assert
    var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
    var response = okResult.Value.Should().BeOfType<ProductResponse>().Subject;
    response.Id.Should().Be(1);
    response.Name.Should().Be("Laptop");
    _mockService.Verify(s => s.GetProductAsync(1), Times.Once);
}

[Fact]
public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
{
    // Arrange
    _mockService.Setup(s => s.GetProductAsync(999))
        .ReturnsAsync((Product?)null);

    // Act
    var result = await _controller.GetById(999);

    // Assert
    result.Result.Should().BeOfType<NotFoundObjectResult>();
}
}