using KMSTraining.API.Application.DTOs;
using KMSTraining.API.Application.Services;
using KMSTraining.API.Controllers;
using KMSTraining.API.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace KMSTraining.Tests.Controllers;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAuthService> _authService = null!;
    private AuthController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _authService = new Mock<IAuthService>();
        _controller = new AuthController(_authService.Object, Mock.Of<ILogger<AuthController>>());
    }

    [Test]
    public async Task Register_ValidInput_ReturnsOkWithToken()
    {
        var registerDto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };

        _authService.Setup(s => s.RegisterAsync(registerDto))
            .ReturnsAsync(new AuthResponseDto
            {
                Token = "test-jwt-token",
                Username = "testuser",
                Email = "test@example.com",
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });

        var result = await _controller.Register(registerDto);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        _authService.Verify(s => s.RegisterAsync(registerDto), Times.Once);
    }

    [Test]
    public async Task Register_DuplicateUsername_ReturnsBadRequest()
    {
        var registerDto = new RegisterDto
        {
            Username = "existinguser",
            Email = "test@example.com",
            Password = "password123"
        };

        _authService.Setup(s => s.RegisterAsync(registerDto))
            .ThrowsAsync(new DuplicateEntityException("Username already exists"));

        var result = await _controller.Register(registerDto);

        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        var loginDto = new LoginDto
        {
            UsernameOrEmail = "testuser",
            Password = "password123"
        };

        _authService.Setup(s => s.LoginAsync(loginDto))
            .ReturnsAsync(new AuthResponseDto
            {
                Token = "test-jwt-token",
                Username = "testuser",
                Email = "test@example.com",
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });

        var result = await _controller.Login(loginDto);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        _authService.Verify(s => s.LoginAsync(loginDto), Times.Once);
    }

    [Test]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var loginDto = new LoginDto
        {
            UsernameOrEmail = "testuser",
            Password = "wrongpassword"
        };

        _authService.Setup(s => s.LoginAsync(loginDto))
            .ThrowsAsync(new AuthenticationException("Invalid credentials"));

        var result = await _controller.Login(loginDto);

        Assert.That(result.Result, Is.InstanceOf<UnauthorizedObjectResult>());
    }
}
