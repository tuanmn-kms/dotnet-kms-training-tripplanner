using KMSTraining.API.Application.DTOs;
using KMSTraining.API.Application.Services;
using KMSTraining.API.Controllers;
using KMSTraining.API.Domain.Entities;
using KMSTraining.API.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace KMSTraining.Tests.Controllers;

[TestFixture]
public class TripsControllerTests
{
    private Mock<ITripService> _tripService = null!;
    private TripsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _tripService = new Mock<ITripService>();
        _controller = new TripsController(_tripService.Object, Mock.Of<ILogger<TripsController>>());

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "7")
        };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
            }
        };
    }

    [Test]
    public async Task GetTrips_ReturnsCurrentUsersTrips()
    {
        var trips = new[]
        {
            new TripDto { Id = 1, Name = "Trip 1", UserId = 7 },
            new TripDto { Id = 2, Name = "Trip 2", UserId = 7 }
        };

        _tripService.Setup(s => s.GetUserTripsAsync(7)).ReturnsAsync(trips);

        var result = await _controller.GetTrips();

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        Assert.That((IEnumerable<TripDto>)okResult.Value!, Has.Exactly(2).Items);
        _tripService.Verify(s => s.GetUserTripsAsync(7), Times.Once);
    }

    [Test]
    public async Task GetTrip_WhenMissing_ReturnsNotFound()
    {
        _tripService.Setup(s => s.GetTripByIdAsync(404)).ReturnsAsync((TripDetailDto?)null);

        var result = await _controller.GetTrip(404);

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task CreateTrip_ReturnsCreatedTrip()
    {
        var request = new CreateTripDto
        {
            Name = "New Trip",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(3)
        };

        _tripService.Setup(s => s.CreateTripAsync(7, request))
            .ReturnsAsync(new TripDto { Id = 10, Name = "New Trip", Status = TripStatus.Planning, UserId = 7 });

        var result = await _controller.CreateTrip(request);

        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = (CreatedAtActionResult)result.Result!;
        Assert.That(((TripDto)createdResult.Value!).Name, Is.EqualTo("New Trip"));
    }

    [Test]
    public async Task UpdateTrip_WhenMissing_ReturnsNotFound()
    {
        _tripService.Setup(s => s.UpdateTripAsync(99, It.IsAny<UpdateTripDto>()))
            .ThrowsAsync(new EntityNotFoundException(nameof(Trip), 99));

        var result = await _controller.UpdateTrip(99, new UpdateTripDto { Name = "Updated" });

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task DeleteTrip_WhenDeleted_ReturnsNoContent()
    {
        _tripService.Setup(s => s.DeleteTripAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteTrip(1);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task ChangeTripStatus_CallsServiceAndReturnsUpdatedTrip()
    {
        _tripService.Setup(s => s.ChangeTripStatusAsync(1, TripStatus.Confirmed)).ReturnsAsync(true);
        _tripService.Setup(s => s.GetTripByIdAsync(1))
            .ReturnsAsync(new TripDetailDto { Id = 1, Name = "Trip", Status = TripStatus.Confirmed, UserId = 7 });

        var result = await _controller.ChangeTripStatus(1, new ChangeStatusRequest { Status = TripStatus.Confirmed });

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        _tripService.Verify(s => s.ChangeTripStatusAsync(1, TripStatus.Confirmed), Times.Once);
    }
}
