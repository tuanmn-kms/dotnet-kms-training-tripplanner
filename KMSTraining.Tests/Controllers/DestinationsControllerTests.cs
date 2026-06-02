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
public class DestinationsControllerTests
{
    private TripPlannerDbContext _context = null!;
    private DestinationsController _controller = null!;
    private User _testUser = null!;
    private Trip _testTrip = null!;

    [SetUp]
    public void SetUp()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _controller = new DestinationsController(_context);

        // Create test user and trip
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
    public async Task GetDestinations_ReturnsAllDestinations()
    {
        // Arrange
        _context.Destinations.AddRange(
            new Destination
            {
                Name = "Paris",
                Country = "France",
                ArrivalDate = DateTime.UtcNow.AddDays(1),
                DepartureDate = DateTime.UtcNow.AddDays(3),
                TripId = _testTrip.Id
            },
            new Destination
            {
                Name = "London",
                Country = "UK",
                ArrivalDate = DateTime.UtcNow.AddDays(4),
                DepartureDate = DateTime.UtcNow.AddDays(6),
                TripId = _testTrip.Id
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetDestinations();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var destinations = okResult!.Value as IEnumerable<DestinationDto>;
        Assert.That(destinations!.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetDestinations_WithTripId_ReturnsFilteredDestinations()
    {
        // Arrange
        var anotherTrip = new Trip
        {
            Name = "Another Trip",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            UserId = _testUser.Id
        };
        _context.Trips.Add(anotherTrip);
        await _context.SaveChangesAsync();

        _context.Destinations.AddRange(
            new Destination
            {
                Name = "Paris",
                Country = "France",
                ArrivalDate = DateTime.UtcNow.AddDays(1),
                DepartureDate = DateTime.UtcNow.AddDays(3),
                TripId = _testTrip.Id
            },
            new Destination
            {
                Name = "Rome",
                Country = "Italy",
                ArrivalDate = DateTime.UtcNow.AddDays(1),
                DepartureDate = DateTime.UtcNow.AddDays(3),
                TripId = anotherTrip.Id
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetDestinations(_testTrip.Id);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var destinations = okResult!.Value as IEnumerable<DestinationDto>;
        Assert.That(destinations!.Count(), Is.EqualTo(1));
        Assert.That(destinations?.First().Name, Is.EqualTo("Paris"));
    }

    [Test]
    public async Task GetDestinations_ExcludesOtherUsersDestinations()
    {
        // Arrange
        var anotherUser = new User
        {
            Id = 99,
            Username = "otheruser",
            Email = "other@example.com",
            PasswordHash = "hash"
        };
        _context.Users.Add(anotherUser);

        var anotherTrip = new Trip
        {
            Id = 99,
            Name = "Other User Trip",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5),
            UserId = anotherUser.Id
        };
        _context.Trips.Add(anotherTrip);

        _context.Destinations.AddRange(
            new Destination
            {
                Name = "My Destination",
                Country = "France",
                ArrivalDate = DateTime.UtcNow.AddDays(1),
                DepartureDate = DateTime.UtcNow.AddDays(2),
                TripId = _testTrip.Id
            },
            new Destination
            {
                Name = "Other Destination",
                Country = "Japan",
                ArrivalDate = DateTime.UtcNow.AddDays(1),
                DepartureDate = DateTime.UtcNow.AddDays(2),
                TripId = anotherTrip.Id
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetDestinations();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var destinations = okResult!.Value as IEnumerable<DestinationDto>;
        Assert.That(destinations!.Count(), Is.EqualTo(1));
        Assert.That(destinations!.First().Name, Is.EqualTo("My Destination"));
    }

    [Test]
    public async Task GetDestination_ExistingDestination_ReturnsDestinationWithActivities()
    {
        // Arrange
        var destination = new Destination
        {
            Name = "Paris",
            Country = "France",
            City = "Paris",
            ArrivalDate = DateTime.UtcNow.AddDays(1),
            DepartureDate = DateTime.UtcNow.AddDays(3),
            TripId = _testTrip.Id
        };
        _context.Destinations.Add(destination);
        await _context.SaveChangesAsync();

        _context.Activities.Add(new Activity
        {
            Name = "Museum Visit",
            ScheduledDateTime = DateTime.UtcNow.AddDays(2),
            DurationMinutes = 90,
            DestinationId = destination.Id
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetDestination(destination.Id);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var destinationDto = okResult!.Value as DestinationDetailDto;
        Assert.That(destinationDto, Is.Not.Null);
        Assert.That(destinationDto!.Name, Is.EqualTo("Paris"));
        Assert.That(destinationDto.Activities.Count, Is.EqualTo(1));
        Assert.That(destinationDto.Activities[0].Name, Is.EqualTo("Museum Visit"));
    }

    [Test]
    public async Task GetDestination_NotFound_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetDestination(99999);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task CreateDestination_ValidDestination_ReturnsCreated()
    {
        // Arrange
        var createDto = new CreateDestinationDto
        {
            Name = "Tokyo",
            Country = "Japan",
            City = "Tokyo",
            ArrivalDate = DateTime.UtcNow.AddDays(1),
            DepartureDate = DateTime.UtcNow.AddDays(4),
            TripId = _testTrip.Id
        };

        // Act
        var result = await _controller.CreateDestination(createDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = result.Result as CreatedAtActionResult;
        var destinationDto = createdResult!.Value as DestinationDto;
        Assert.That(destinationDto!.Name, Is.EqualTo("Tokyo"));
    }

    [Test]
    public async Task CreateDestination_DepartureDateBeforeArrival_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateDestinationDto
        {
            Name = "Invalid",
            Country = "Test",
            ArrivalDate = DateTime.UtcNow.AddDays(5),
            DepartureDate = DateTime.UtcNow.AddDays(2),
            TripId = _testTrip.Id
        };

        // Act
        var result = await _controller.CreateDestination(createDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task CreateDestination_TripNotFound_ReturnsNotFound()
    {
        // Arrange
        var createDto = new CreateDestinationDto
        {
            Name = "Unknown Trip Destination",
            Country = "Test",
            ArrivalDate = DateTime.UtcNow.AddDays(1),
            DepartureDate = DateTime.UtcNow.AddDays(2),
            TripId = 99999
        };

        // Act
        var result = await _controller.CreateDestination(createDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task CreateDestination_DatesOutsideTripDates_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateDestinationDto
        {
            Name = "Invalid",
            Country = "Test",
            ArrivalDate = DateTime.UtcNow.AddDays(-5),
            DepartureDate = DateTime.UtcNow.AddDays(-2),
            TripId = _testTrip.Id
        };

        // Act
        var result = await _controller.CreateDestination(createDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateDestination_ValidUpdate_ReturnsUpdated()
    {
        // Arrange
        var destination = new Destination
        {
            Name = "Paris",
            Country = "France",
            ArrivalDate = DateTime.UtcNow.AddDays(1),
            DepartureDate = DateTime.UtcNow.AddDays(3),
            TripId = _testTrip.Id
        };
        _context.Destinations.Add(destination);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateDestinationDto
        {
            Name = "Paris Updated",
            City = "Paris"
        };

        // Act
        var result = await _controller.UpdateDestination(destination.Id, updateDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var destinationDto = okResult!.Value as DestinationDto;
        Assert.That(destinationDto!.Name, Is.EqualTo("Paris Updated"));
        Assert.That(destinationDto.City, Is.EqualTo("Paris"));
    }

    [Test]
    public async Task UpdateDestination_NotFound_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateDestinationDto
        {
            Name = "Updated"
        };

        // Act
        var result = await _controller.UpdateDestination(99999, updateDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task UpdateDestination_DepartureBeforeArrival_ReturnsBadRequest()
    {
        // Arrange
        var destination = new Destination
        {
            Name = "Paris",
            Country = "France",
            ArrivalDate = DateTime.UtcNow.AddDays(1),
            DepartureDate = DateTime.UtcNow.AddDays(3),
            TripId = _testTrip.Id
        };
        _context.Destinations.Add(destination);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateDestinationDto
        {
            ArrivalDate = DateTime.UtcNow.AddDays(5),
            DepartureDate = DateTime.UtcNow.AddDays(4)
        };

        // Act
        var result = await _controller.UpdateDestination(destination.Id, updateDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task DeleteDestination_ExistingDestination_ReturnsNoContent()
    {
        // Arrange
        var destination = new Destination
        {
            Name = "To Delete",
            Country = "Test",
            ArrivalDate = DateTime.UtcNow.AddDays(1),
            DepartureDate = DateTime.UtcNow.AddDays(3),
            TripId = _testTrip.Id
        };
        _context.Destinations.Add(destination);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteDestination(destination.Id);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task DeleteDestination_NotFound_ReturnsNotFound()
    {
        // Act
        var result = await _controller.DeleteDestination(99999);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }
}
