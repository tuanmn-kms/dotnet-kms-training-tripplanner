using KMSTraining.API.Application.DTOs;
using KMSTraining.API.Application.Services;
using KMSTraining.API.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMSTraining.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ActivitiesController : ControllerBase
{
    private readonly IActivityService _activityService;
    private readonly ILogger<ActivitiesController> _logger;

    public ActivitiesController(IActivityService activityService, ILogger<ActivitiesController> logger)
    {
        _activityService = activityService;
        _logger = logger;
    }


    /// <summary>
    /// Get all activities for a destination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivities([FromQuery] int? destinationId = null)
    {
        try
        {
            _logger.LogInformation("Fetching activities for destination {DestinationId}", destinationId);
            var activities = await _activityService.GetDestinationActivitiesAsync(destinationId ?? 0);
            return Ok(activities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching activities for destination {DestinationId}", destinationId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error fetching activities" });
        }
    }

    /// <summary>
    /// Get a specific activity
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActivityDto>> GetActivity(int id)
    {
        try
        {
            _logger.LogInformation("Fetching activity {ActivityId}", id);
            var activity = await _activityService.GetActivityByIdAsync(id);

            if (activity == null)
            {
                return NotFound(new { message = "Activity not found" });
            }
            
            return Ok(activity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching activity {ActivityId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error fetching activity" });
        }
    }

    /// <summary>
    /// Create a new activity
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ActivityDto>> CreateActivity([FromBody] CreateActivityDto createActivityDto)
    {
        try
        {
            _logger.LogInformation("Creating activity for destination {DestinationId}", createActivityDto.DestinationId);
            var activity = await _activityService.CreateActivityAsync(createActivityDto.DestinationId, createActivityDto);
            return CreatedAtAction(nameof(GetActivity), new { id = activity.Id }, activity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating activity");
            return BadRequest(new { message = "Error creating activity" });
        }
    }

    /// <summary>
    /// Update an activity
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActivityDto>> UpdateActivity(int id, [FromBody] UpdateActivityDto updateActivityDto)
    {
        try
        {
            _logger.LogInformation("Updating activity {ActivityId}", id);
            var activity = await _activityService.UpdateActivityAsync(id, updateActivityDto);
            return Ok(activity);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning("Activity not found: {ActivityId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating activity {ActivityId}", id);
            return BadRequest(new { message = "Error updating activity" });
        }
    }

    /// <summary>
    /// Delete an activity
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteActivity(int id)
    {
        try
        {
            _logger.LogInformation("Deleting activity {ActivityId}", id);
            var result = await _activityService.DeleteActivityAsync(id);
            
            if (!result)
            {
                return NotFound(new { message = "Activity not found" });
            }
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting activity {ActivityId}", id);
            return BadRequest(new { message = "Error deleting activity" });
        }
    }
}
