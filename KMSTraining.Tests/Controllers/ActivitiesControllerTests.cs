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
public class ActivitiesControllerTests
{
    private TripPlannerDbContext _context = null!;
    private ActivitiesController _controller = null!;
    private User _testUser = null!;
    private Trip _testTrip = null!;
    private Destination _testDestination = null!;

    [SetUp]
    public void SetUp()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _controller = new ActivitiesController(_context);

        // Create test data
        _testUser = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash"
        };
        _context.Users.Add(_testUser);

        _testTrip = new Trip
        {
            Id = 1,
            Name = "Test Trip",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            UserId = _testUser.Id
        };
        _context.Trips.Add(_testTrip);

        _testDestination = new Destination
        {
            Id = 1,
            Name = "Paris",
            Country = "France",
            ArrivalDate = DateTime.UtcNow.AddDays(1),
            DepartureDate = DateTime.UtcNow.AddDays(5),
            TripId = _testTrip.Id
        };
        _context.Destinations.Add(_testDestination);
        _context.SaveChanges();

        // Setup claims
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
    public async Task GetActivities_ReturnsAllActivities()
    {
        // Arrange
        _context.Activities.AddRange(
            new Activity
            {
                Name = "Eiffel Tower Visit",
                ScheduledDateTime = DateTime.UtcNow.AddDays(2),
                DurationMinutes = 120,
                DestinationId = _testDestination.Id
            },
            new Activity
            {
                Name = "Louvre Museum",
                ScheduledDateTime = DateTime.UtcNow.AddDays(3),
                DurationMinutes = 180,
                DestinationId = _testDestination.Id
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetActivities();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var activities = okResult!.Value as IEnumerable<ActivityDto>;
        Assert.That(activities!.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetActivities_WithDestinationId_ReturnsFilteredActivities()
    {
        // Arrange
        var anotherDestination = new Destination
        {
            Name = "London",
            Country = "UK",
            ArrivalDate = DateTime.UtcNow.AddDays(6),
            DepartureDate = DateTime.UtcNow.AddDays(9),
            TripId = _testTrip.Id
        };
        _context.Destinations.Add(anotherDestination);
        await _context.SaveChangesAsync();

        _context.Activities.AddRange(
            new Activity
            {
                Name = "Eiffel Tower",
                ScheduledDateTime = DateTime.UtcNow.AddDays(2),
                DurationMinutes = 120,
                DestinationId = _testDestination.Id
            },
            new Activity
            {
                Name = "Big Ben",
                ScheduledDateTime = DateTime.UtcNow.AddDays(7),
                DurationMinutes = 60,
                DestinationId = anotherDestination.Id
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetActivities(_testDestination.Id);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var activities = okResult!.Value as IEnumerable<ActivityDto>;
        Assert.That(activities!.Count(), Is.EqualTo(1));
        Assert.That(activities.First().Name, Is.EqualTo("Eiffel Tower"));
    }

    [Test]
    public async Task CreateActivity_ValidActivity_ReturnsCreated()
    {
        // Arrange
        var createDto = new CreateActivityDto
        {
            Name = "Seine River Cruise",
            Description = "Evening cruise",
            ScheduledDateTime = DateTime.UtcNow.AddDays(2),
            DurationMinutes = 90,
            Location = "Seine River",
            EstimatedCost = 50.00m,
            DestinationId = _testDestination.Id
        };

        // Act
        var result = await _controller.CreateActivity(createDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = result.Result as CreatedAtActionResult;
        var activityDto = createdResult!.Value as ActivityDto;
        Assert.That(activityDto!.Name, Is.EqualTo("Seine River Cruise"));
        Assert.That(activityDto.EstimatedCost, Is.EqualTo(50.00m));
    }

    [Test]
    public async Task CreateActivity_ScheduledOutsideDestinationDates_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateActivityDto
        {
            Name = "Invalid Activity",
            ScheduledDateTime = DateTime.UtcNow.AddDays(10), // Outside destination dates
            DurationMinutes = 60,
            DestinationId = _testDestination.Id
        };

        // Act
        var result = await _controller.CreateActivity(createDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task CreateActivity_NonExistentDestination_ReturnsNotFound()
    {
        // Arrange
        var createDto = new CreateActivityDto
        {
            Name = "Activity",
            ScheduledDateTime = DateTime.UtcNow.AddDays(2),
            DurationMinutes = 60,
            DestinationId = 999
        };

        // Act
        var result = await _controller.CreateActivity(createDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task UpdateActivity_ValidUpdate_ReturnsUpdated()
    {
        // Arrange
        var activity = new Activity
        {
            Name = "Original Activity",
            ScheduledDateTime = DateTime.UtcNow.AddDays(2),
            DurationMinutes = 60,
            DestinationId = _testDestination.Id
        };
        _context.Activities.Add(activity);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateActivityDto
        {
            Name = "Updated Activity",
            DurationMinutes = 120,
            EstimatedCost = 75.00m
        };

        // Act
        var result = await _controller.UpdateActivity(activity.Id, updateDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var activityDto = okResult!.Value as ActivityDto;
        Assert.That(activityDto!.Name, Is.EqualTo("Updated Activity"));
        Assert.That(activityDto.DurationMinutes, Is.EqualTo(120));
        Assert.That(activityDto.EstimatedCost, Is.EqualTo(75.00m));
    }

    [Test]
    public async Task DeleteActivity_ExistingActivity_ReturnsNoContent()
    {
        // Arrange
        var activity = new Activity
        {
            Name = "To Delete",
            ScheduledDateTime = DateTime.UtcNow.AddDays(2),
            DurationMinutes = 60,
            DestinationId = _testDestination.Id
        };
        _context.Activities.Add(activity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteActivity(activity.Id);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());

        var deletedActivity = await _context.Activities.FindAsync(activity.Id);
        Assert.That(deletedActivity, Is.Null);
    }

    [Test]
    public async Task GetActivity_ExistingActivity_ReturnsActivity()
    {
        // Arrange
        var activity = new Activity
        {
            Name = "Test Activity",
            Description = "Test Description",
            ScheduledDateTime = DateTime.UtcNow.AddDays(2),
            DurationMinutes = 90,
            Location = "Test Location",
            EstimatedCost = 100.00m,
            DestinationId = _testDestination.Id
        };
        _context.Activities.Add(activity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetActivity(activity.Id);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var activityDto = okResult!.Value as ActivityDto;
        Assert.That(activityDto!.Name, Is.EqualTo("Test Activity"));
        Assert.That(activityDto.Location, Is.EqualTo("Test Location"));
    }
}
