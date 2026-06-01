using KMSTraining.API.DTOs;
using KMSTraining.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace KMSTraining.Tests.Services;

[TestFixture]
public class TokenServiceTests
{
    private ITokenService _tokenService = null!;
    private Mock<IConfiguration> _mockConfiguration = null!;

    [SetUp]
    public void SetUp()
    {
        _mockConfiguration = new Mock<IConfiguration>();

        var jwtSettings = new Dictionary<string, string>
        {
            { "JwtSettings:SecretKey", "TestSecretKeyForJWTTokenGeneration123456789" },
            { "JwtSettings:Issuer", "TestIssuer" },
            { "JwtSettings:Audience", "TestAudience" },
            { "JwtSettings:ExpirationMinutes", "60" }
        };

        _mockConfiguration.Setup(c => c.GetSection("JwtSettings")["SecretKey"])
            .Returns(jwtSettings["JwtSettings:SecretKey"]);
        _mockConfiguration.Setup(c => c.GetSection("JwtSettings")["Issuer"])
            .Returns(jwtSettings["JwtSettings:Issuer"]);
        _mockConfiguration.Setup(c => c.GetSection("JwtSettings")["Audience"])
            .Returns(jwtSettings["JwtSettings:Audience"]);
        _mockConfiguration.Setup(c => c.GetSection("JwtSettings")["ExpirationMinutes"])
            .Returns(jwtSettings["JwtSettings:ExpirationMinutes"]);

        _tokenService = new TokenService(_mockConfiguration.Object);
    }

    [Test]
    public void GenerateToken_ValidInput_ReturnsJwtToken()
    {
        // Arrange
        int userId = 1;
        string username = "testuser";
        string email = "test@example.com";

        // Act
        var token = _tokenService.GenerateToken(userId, username, email);

        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.Not.Empty);
        Assert.That(token.Split('.').Length, Is.EqualTo(3)); // JWT has 3 parts
    }

    [Test]
    public void GenerateToken_DifferentUsers_ReturnsDifferentTokens()
    {
        // Arrange
        var token1 = _tokenService.GenerateToken(1, "user1", "user1@example.com");
        var token2 = _tokenService.GenerateToken(2, "user2", "user2@example.com");

        // Assert
        Assert.That(token1, Is.Not.EqualTo(token2));
    }
}
