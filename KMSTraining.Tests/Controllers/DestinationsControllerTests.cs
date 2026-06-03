using KMSTraining.API.Application.DTOs;
using KMSTraining.API.Application.Services;
using KMSTraining.API.Controllers;
using KMSTraining.API.Domain.Entities;
using KMSTraining.API.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace KMSTraining.Tests.Controllers;

[TestFixture]
public class DestinationsControllerTests
{
    private Mock<IDestinationService> _destinationService = null!;
    private DestinationsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _destinationService = new Mock<IDestinationService>();
        _controller = new DestinationsController(_destinationService.Object, Mock.Of<ILogger<DestinationsController>>());
    }

    [Test]
    public async Task GetDestinations_WithTripId_ReturnsFilteredDestinations()
    {
        var destinations = new[]
        {
            new DestinationDto { Id = 1, Name = "Paris", Country = "France", TripId = 5 }
        };

        _destinationService.Setup(s => s.GetTripDestinationsAsync(5)).ReturnsAsync(destinations);

        var result = await _controller.GetDestinations(5);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        _destinationService.Verify(s => s.GetTripDestinationsAsync(5), Times.Once);
    }

    [Test]
    public async Task GetDestination_WhenMissing_ReturnsNotFound()
    {
        _destinationService.Setup(s => s.GetDestinationByIdAsync(404)).ReturnsAsync((DestinationDetailDto?)null);

        var result = await _controller.GetDestination(404);

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task CreateDestination_UsesDtoTripIdAndReturnsCreated()
    {
        var request = new CreateDestinationDto
        {
            Name = "Tokyo",
            Country = "Japan",
            ArrivalDate = DateTime.UtcNow,
            DepartureDate = DateTime.UtcNow.AddDays(3),
            TripId = 9
        };

        _destinationService.Setup(s => s.CreateDestinationAsync(9, request))
            .ReturnsAsync(new DestinationDto { Id = 12, Name = "Tokyo", Country = "Japan", TripId = 9 });

        var result = await _controller.CreateDestination(request);

        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        _destinationService.Verify(s => s.CreateDestinationAsync(9, request), Times.Once);
    }

    [Test]
    public async Task UpdateDestination_WhenMissing_ReturnsNotFound()
    {
        _destinationService.Setup(s => s.UpdateDestinationAsync(404, It.IsAny<UpdateDestinationDto>()))
            .ThrowsAsync(new EntityNotFoundException(nameof(Destination), 404));

        var result = await _controller.UpdateDestination(404, new UpdateDestinationDto { Name = "Updated" });

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task DeleteDestination_WhenDeleted_ReturnsNoContent()
    {
        _destinationService.Setup(s => s.DeleteDestinationAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteDestination(1);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}
