using KMSTraining.API.Data;
using KMSTraining.API.DTOs;
using KMSTraining.API.Models;
using KMSTraining.API.Services;
using KMSTraining.Tests.Helpers;
using Moq;

namespace KMSTraining.Tests.Services;

[TestFixture]
public class AuthServiceTests
{
    private TripPlannerDbContext _context = null!;
    private Mock<ITokenService> _mockTokenService = null!;
    private IAuthService _authService = null!;

    [SetUp]
    public void SetUp()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _mockTokenService = new Mock<ITokenService>();
        _authService = new AuthService(_context, _mockTokenService.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task RegisterAsync_ValidUser_ReturnsAuthResponse()
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

        _mockTokenService.Setup(t => t.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("test-jwt-token");

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.EqualTo("test-jwt-token"));
        Assert.That(result.Username, Is.EqualTo("testuser"));
        Assert.That(result.Email, Is.EqualTo("test@example.com"));

        var userInDb = await _context.Users.FindAsync(1);
        Assert.That(userInDb, Is.Not.Null);
        Assert.That(userInDb.Username, Is.EqualTo("testuser"));
    }

    [Test]
    public void RegisterAsync_DuplicateUsername_ThrowsInvalidOperationException()
    {
        // Arrange
        _context.Users.Add(new User
        {
            Username = "existinguser",
            Email = "existing@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password")
        });
        _context.SaveChanges();

        var registerDto = new RegisterDto
        {
            Username = "existinguser",
            Email = "newemail@example.com",
            Password = "password123"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _authService.RegisterAsync(registerDto));
        Assert.That(ex.Message, Is.EqualTo("Username already exists"));
    }

    [Test]
    public void RegisterAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        _context.Users.Add(new User
        {
            Username = "existinguser",
            Email = "existing@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password")
        });
        _context.SaveChanges();

        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "existing@example.com",
            Password = "password123"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _authService.RegisterAsync(registerDto));
        Assert.That(ex.Message, Is.EqualTo("Email already exists"));
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        _context.Users.Add(new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = passwordHash
        });
        await _context.SaveChangesAsync();

        _mockTokenService.Setup(t => t.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("test-jwt-token");

        var loginDto = new LoginDto
        {
            UsernameOrEmail = "testuser",
            Password = "password123"
        };

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.EqualTo("test-jwt-token"));
        Assert.That(result.Username, Is.EqualTo("testuser"));
    }

    [Test]
    public void LoginAsync_InvalidUsername_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            UsernameOrEmail = "nonexistent",
            Password = "password123"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _authService.LoginAsync(loginDto));
        Assert.That(ex.Message, Is.EqualTo("Invalid credentials"));
    }

    [Test]
    public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        _context.Users.Add(new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = passwordHash
        });
        await _context.SaveChangesAsync();

        var loginDto = new LoginDto
        {
            UsernameOrEmail = "testuser",
            Password = "wrongpassword"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _authService.LoginAsync(loginDto));
        Assert.That(ex.Message, Is.EqualTo("Invalid credentials"));
    }

    [Test]
    public async Task LoginAsync_EmailAsUsername_ReturnsAuthResponse()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        _context.Users.Add(new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = passwordHash
        });
        await _context.SaveChangesAsync();

        _mockTokenService.Setup(t => t.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("test-jwt-token");

        var loginDto = new LoginDto
        {
            UsernameOrEmail = "test@example.com",
            Password = "password123"
        };

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.EqualTo("test-jwt-token"));
    }
}
