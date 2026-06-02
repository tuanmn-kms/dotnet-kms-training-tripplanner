using KMSTraining.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

namespace KMSTraining.Tests.Controllers;

[TestFixture]
public class HealthControllerTests
{
    private Mock<HealthCheckService> _mockHealthCheckService = null!;
    private HealthController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _mockHealthCheckService = new Mock<HealthCheckService>();
        _controller = new HealthController(_mockHealthCheckService.Object);
    }

    [Test]
    public async Task Get_WhenHealthy_ReturnsOkWithHealthStatus()
    {
        // Arrange
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["database"] = new HealthReportEntry(
                    HealthStatus.Healthy,
                    "Database is healthy",
                    TimeSpan.FromMilliseconds(100),
                    null,
                    null
                )
            },
            TimeSpan.FromMilliseconds(150)
        );

        _mockHealthCheckService
            .Setup(s => s.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthReport);

        // Act
        var result = await _controller.Get();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));

        // Verify the response contains expected properties
        var response = okResult.Value;
        Assert.That(response, Is.Not.Null);

        var statusProperty = response!.GetType().GetProperty("status");
        Assert.That(statusProperty, Is.Not.Null);
        Assert.That(statusProperty!.GetValue(response), Is.EqualTo("Healthy"));
    }

    [Test]
    public async Task Get_WhenUnhealthy_ReturnsServiceUnavailable()
    {
        // Arrange
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["database"] = new HealthReportEntry(
                    HealthStatus.Unhealthy,
                    "Database connection failed",
                    TimeSpan.FromMilliseconds(100),
                    new Exception("Connection timeout"),
                    null
                )
            },
            TimeSpan.FromMilliseconds(150)
        );

        _mockHealthCheckService
            .Setup(s => s.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthReport);

        // Act
        var result = await _controller.Get();

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(503));

        var response = objectResult.Value;
        Assert.That(response, Is.Not.Null);

        var statusProperty = response!.GetType().GetProperty("status");
        Assert.That(statusProperty, Is.Not.Null);
        Assert.That(statusProperty!.GetValue(response), Is.EqualTo("Unhealthy"));
    }

    [Test]
    public async Task Get_WhenDegraded_ReturnsServiceUnavailable()
    {
        // Arrange
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["database"] = new HealthReportEntry(
                    HealthStatus.Degraded,
                    "Database response slow",
                    TimeSpan.FromMilliseconds(5000),
                    null,
                    null
                )
            },
            TimeSpan.FromMilliseconds(5100)
        );

        _mockHealthCheckService
            .Setup(s => s.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthReport);

        // Act
        var result = await _controller.Get();

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(503));
    }

    [Test]
    public async Task Get_ReturnsChecksInformation()
    {
        // Arrange
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["database"] = new HealthReportEntry(
                    HealthStatus.Healthy,
                    "Database is healthy",
                    TimeSpan.FromMilliseconds(100),
                    null,
                    new Dictionary<string, object> { ["server"] = "localhost" }
                ),
                ["memory"] = new HealthReportEntry(
                    HealthStatus.Healthy,
                    "Memory usage is normal",
                    TimeSpan.FromMilliseconds(50),
                    null,
                    null
                )
            },
            TimeSpan.FromMilliseconds(150)
        );

        _mockHealthCheckService
            .Setup(s => s.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthReport);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var response = okResult!.Value;
        var checksProperty = response!.GetType().GetProperty("checks");
        Assert.That(checksProperty, Is.Not.Null);

        var checks = checksProperty!.GetValue(response) as IEnumerable<object>;
        Assert.That(checks, Is.Not.Null);
        Assert.That(checks!.Count(), Is.EqualTo(2));
    }

    [Test]
    public void Live_ReturnsOkWithAliveStatus()
    {
        // Act
        var result = _controller.Live();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));

        var response = okResult.Value;
        Assert.That(response, Is.Not.Null);

        var statusProperty = response!.GetType().GetProperty("status");
        Assert.That(statusProperty, Is.Not.Null);
        Assert.That(statusProperty!.GetValue(response), Is.EqualTo("Alive"));

        var timestampProperty = response.GetType().GetProperty("timestamp");
        Assert.That(timestampProperty, Is.Not.Null);
        var timestamp = (DateTime)timestampProperty!.GetValue(response)!;
        Assert.That(timestamp, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public async Task Ready_WhenHealthy_ReturnsOkWithReadyStatus()
    {
        // Arrange
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["database"] = new HealthReportEntry(
                    HealthStatus.Healthy,
                    "Ready",
                    TimeSpan.FromMilliseconds(50),
                    null,
                    null
                )
            },
            TimeSpan.FromMilliseconds(50)
        );

        _mockHealthCheckService
            .Setup(s => s.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthReport);

        // Act
        var result = await _controller.Ready();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));

        var response = okResult.Value;
        var statusProperty = response!.GetType().GetProperty("status");
        Assert.That(statusProperty!.GetValue(response), Is.EqualTo("Ready"));
    }

    [Test]
    public async Task Ready_WhenUnhealthy_ReturnsServiceUnavailable()
    {
        // Arrange
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["database"] = new HealthReportEntry(
                    HealthStatus.Unhealthy,
                    "Not Ready",
                    TimeSpan.FromMilliseconds(50),
                    null,
                    null
                )
            },
            TimeSpan.FromMilliseconds(50)
        );

        _mockHealthCheckService
            .Setup(s => s.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthReport);

        // Act
        var result = await _controller.Ready();

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(503));

        var response = objectResult.Value;
        var statusProperty = response!.GetType().GetProperty("status");
        Assert.That(statusProperty!.GetValue(response), Is.EqualTo("NotReady"));
    }

    [Test]
    public async Task Ready_WhenDegraded_ReturnsServiceUnavailable()
    {
        // Arrange
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["database"] = new HealthReportEntry(
                    HealthStatus.Degraded,
                    "Degraded",
                    TimeSpan.FromMilliseconds(50),
                    null,
                    null
                )
            },
            TimeSpan.FromMilliseconds(50)
        );

        _mockHealthCheckService
            .Setup(s => s.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthReport);

        // Act
        var result = await _controller.Ready();

        // Assert
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(503));
    }
}
