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
public class ActivitiesControllerTests
{
    private Mock<IActivityService> _activityService = null!;
    private ActivitiesController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _activityService = new Mock<IActivityService>();
        _controller = new ActivitiesController(_activityService.Object, Mock.Of<ILogger<ActivitiesController>>());
    }

    [Test]
    public async Task GetActivities_WithDestinationId_ReturnsActivities()
    {
        var activities = new[]
        {
            new ActivityDto { Id = 1, Name = "Museum", DestinationId = 3 }
        };

        _activityService.Setup(s => s.GetDestinationActivitiesAsync(3)).ReturnsAsync(activities);

        var result = await _controller.GetActivities(3);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        _activityService.Verify(s => s.GetDestinationActivitiesAsync(3), Times.Once);
    }

    [Test]
    public async Task GetActivity_WhenMissing_ReturnsNotFound()
    {
        _activityService.Setup(s => s.GetActivityByIdAsync(404)).ReturnsAsync((ActivityDto?)null);

        var result = await _controller.GetActivity(404);

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task CreateActivity_UsesDtoDestinationIdAndReturnsCreated()
    {
        var request = new CreateActivityDto
        {
            Name = "Cruise",
            ScheduledDateTime = DateTime.UtcNow,
            DestinationId = 8
        };

        _activityService.Setup(s => s.CreateActivityAsync(8, request))
            .ReturnsAsync(new ActivityDto { Id = 20, Name = "Cruise", DestinationId = 8 });

        var result = await _controller.CreateActivity(request);

        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        _activityService.Verify(s => s.CreateActivityAsync(8, request), Times.Once);
    }

    [Test]
    public async Task UpdateActivity_WhenMissing_ReturnsNotFound()
    {
        _activityService.Setup(s => s.UpdateActivityAsync(404, It.IsAny<UpdateActivityDto>()))
            .ThrowsAsync(new EntityNotFoundException(nameof(Activity), 404));

        var result = await _controller.UpdateActivity(404, new UpdateActivityDto { Name = "Updated" });

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task DeleteActivity_WhenDeleted_ReturnsNoContent()
    {
        _activityService.Setup(s => s.DeleteActivityAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteActivity(1);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}
