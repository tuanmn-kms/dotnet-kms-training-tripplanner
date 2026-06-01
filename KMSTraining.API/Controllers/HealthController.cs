using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KMSTraining.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    /// Get detailed health status of the API and its dependencies
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var report = await _healthCheckService.CheckHealthAsync();

        var result = new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.ToString(),
                exception = e.Value.Exception?.Message,
                data = e.Value.Data
            })
        };

        return report.Status == HealthStatus.Healthy
            ? Ok(result)
            : StatusCode(503, result); // Service Unavailable
    }

    /// <summary>
    /// Simple liveness probe - returns 200 if API is running
    /// </summary>
    [HttpGet("live")]
    public IActionResult Live()
    {
        return Ok(new { status = "Alive", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Readiness probe - returns 200 if API is ready to handle requests
    /// </summary>
    [HttpGet("ready")]
    public async Task<IActionResult> Ready()
    {
        var report = await _healthCheckService.CheckHealthAsync();

        return report.Status == HealthStatus.Healthy
            ? Ok(new { status = "Ready", timestamp = DateTime.UtcNow })
            : StatusCode(503, new { status = "NotReady", timestamp = DateTime.UtcNow });
    }
}
