using KMSTraining.API.Controllers;
using KMSTraining.API.DTOs;
using KMSTraining.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KMSTraining.Tests.Controllers;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAuthService> _mockAuthService = null!;
    private AuthController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
    }

    [Test]
    public async Task Register_ValidInput_ReturnsOkWithToken()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            FirstName = "Test",
            LastName = "User"
        };

        var expectedResponse = new AuthResponseDto
        {
            Token = "test-jwt-token",
            Username = "testuser",
            Email = "test@example.com",
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };

        _mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<RegisterDto>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var authResponse = okResult!.Value as AuthResponseDto;
        Assert.That(authResponse!.Token, Is.EqualTo("test-jwt-token"));
        Assert.That(authResponse.Username, Is.EqualTo("testuser"));
    }

    [Test]
    public async Task Register_DuplicateUsername_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "existinguser",
            Email = "test@example.com",
            Password = "password123"
        };

        _mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<RegisterDto>()))
            .ThrowsAsync(new InvalidOperationException("Username already exists"));

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            UsernameOrEmail = "testuser",
            Password = "password123"
        };

        var expectedResponse = new AuthResponseDto
        {
            Token = "test-jwt-token",
            Username = "testuser",
            Email = "test@example.com",
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };

        _mockAuthService.Setup(s => s.LoginAsync(It.IsAny<LoginDto>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var authResponse = okResult!.Value as AuthResponseDto;
        Assert.That(authResponse!.Token, Is.EqualTo("test-jwt-token"));
    }

    [Test]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            UsernameOrEmail = "testuser",
            Password = "wrongpassword"
        };

        _mockAuthService.Setup(s => s.LoginAsync(It.IsAny<LoginDto>()))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    [Test]
    public async Task Register_ServiceCalled_InvokesAuthService()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };

        var expectedResponse = new AuthResponseDto
        {
            Token = "test-jwt-token",
            Username = "testuser",
            Email = "test@example.com",
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };

        _mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<RegisterDto>()))
            .ReturnsAsync(expectedResponse);

        // Act
        await _controller.Register(registerDto);

        // Assert
        _mockAuthService.Verify(s => s.RegisterAsync(It.Is<RegisterDto>(
            dto => dto.Username == "testuser" && dto.Email == "test@example.com"
        )), Times.Once);
    }

    [Test]
    public async Task Login_ServiceCalled_InvokesAuthService()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            UsernameOrEmail = "testuser",
            Password = "password123"
        };

        var expectedResponse = new AuthResponseDto
        {
            Token = "test-jwt-token",
            Username = "testuser",
            Email = "test@example.com",
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };

        _mockAuthService.Setup(s => s.LoginAsync(It.IsAny<LoginDto>()))
            .ReturnsAsync(expectedResponse);

        // Act
        await _controller.Login(loginDto);

        // Assert
        _mockAuthService.Verify(s => s.LoginAsync(It.Is<LoginDto>(
            dto => dto.UsernameOrEmail == "testuser"
        )), Times.Once);
    }
}
