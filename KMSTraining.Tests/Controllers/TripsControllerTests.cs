using KMSTraining.API.Controllers;
using KMSTraining.API.Data;
using KMSTraining.API.DTOs;
using KMSTraining.API.Models;
using KMSTraining.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMSTraining.Tests.Controllers;

[TestFixture]
public class TripsControllerTests
{
    private TripPlannerDbContext _context = null!;
    private TripsController _controller = null!;
    private User _testUser = null!;

    [SetUp]
    public void SetUp()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _controller = new TripsController(_context);

        // Create test user
        _testUser = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash"
        };
        _context.Users.Add(_testUser);
        _context.SaveChanges();

        // Setup claims for authenticated user
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _testUser.Id.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetTrips_ReturnsUserTrips()
    {
        // Arrange
        _context.Trips.AddRange(
            new Trip { Name = "Trip 1", UserId = _testUser.Id, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(5) },
            new Trip { Name = "Trip 2", UserId = _testUser.Id, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(3) }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetTrips();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var trips = okResult!.Value as IEnumerable<TripDto>;
        Assert.That(trips!.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetTrip_ExistingTrip_ReturnsTrip()
    {
        // Arrange
        var trip = new Trip
        {
            Name = "Test Trip",
            Description = "Test Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5),
            UserId = _testUser.Id
        };
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetTrip(trip.Id);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var tripDto = okResult!.Value as TripDetailDto;
        Assert.That(tripDto, Is.Not.Null);
        Assert.That(tripDto!.Name, Is.EqualTo("Test Trip"));
    }

    [Test]
    public async Task GetTrip_NonExistingTrip_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetTrip(999);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task CreateTrip_ValidTrip_ReturnsCreatedTrip()
    {
        // Arrange
        var createTripDto = new CreateTripDto
        {
            Name = "New Trip",
            Description = "New Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var result = await _controller.CreateTrip(createTripDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = result.Result as CreatedAtActionResult;
        var tripDto = createdResult!.Value as TripDto;
        Assert.That(tripDto, Is.Not.Null);
        Assert.That(tripDto!.Name, Is.EqualTo("New Trip"));
        Assert.That(tripDto.Status, Is.EqualTo(TripStatus.Planning));
    }

    [Test]
    public async Task CreateTrip_EndDateBeforeStartDate_ReturnsBadRequest()
    {
        // Arrange
        var createTripDto = new CreateTripDto
        {
            Name = "Invalid Trip",
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow
        };

        // Act
        var result = await _controller.CreateTrip(createTripDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateTrip_ValidUpdate_ReturnsUpdatedTrip()
    {
        // Arrange
        var trip = new Trip
        {
            Name = "Original Trip",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5),
            UserId = _testUser.Id
        };
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateTripDto
        {
            Name = "Updated Trip",
            Status = TripStatus.Confirmed
        };

        // Act
        var result = await _controller.UpdateTrip(trip.Id, updateDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var tripDto = okResult!.Value as TripDto;
        Assert.That(tripDto!.Name, Is.EqualTo("Updated Trip"));
        Assert.That(tripDto.Status, Is.EqualTo(TripStatus.Confirmed));
    }

    [Test]
    public async Task UpdateTrip_InvalidStatus_ReturnsBadRequest()
    {
        // Arrange
        var trip = new Trip
        {
            Name = "Test Trip",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5),
            UserId = _testUser.Id
        };
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateTripDto
        {
            Status = "InvalidStatus"
        };

        // Act
        var result = await _controller.UpdateTrip(trip.Id, updateDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task DeleteTrip_ExistingTrip_ReturnsNoContent()
    {
        // Arrange
        var trip = new Trip
        {
            Name = "Trip to Delete",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5),
            UserId = _testUser.Id
        };
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteTrip(trip.Id);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());

        var deletedTrip = await _context.Trips.FindAsync(trip.Id);
        Assert.That(deletedTrip, Is.Null);
    }

    [Test]
    public async Task DeleteTrip_NonExistingTrip_ReturnsNotFound()
    {
        // Act
        var result = await _controller.DeleteTrip(999);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }
}
