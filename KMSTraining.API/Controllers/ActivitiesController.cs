using KMSTraining.API.Data;
using KMSTraining.API.DTOs;
using KMSTraining.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KMSTraining.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ActivitiesController : ControllerBase
{
    private readonly TripPlannerDbContext _context;

    public ActivitiesController(TripPlannerDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivities([FromQuery] int? destinationId = null)
    {
        var userId = GetCurrentUserId();
        var query = _context.Activities
            .Include(a => a.Destination)
                .ThenInclude(d => d.Trip)
            .Where(a => a.Destination.Trip.UserId == userId);

        if (destinationId.HasValue)
        {
            query = query.Where(a => a.DestinationId == destinationId.Value);
        }

        var activities = await query
            .Select(a => new ActivityDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                ScheduledDateTime = a.ScheduledDateTime,
                DurationMinutes = a.DurationMinutes,
                Location = a.Location,
                EstimatedCost = a.EstimatedCost,
                DestinationId = a.DestinationId,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();

        return Ok(activities);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ActivityDto>> GetActivity(int id)
    {
        var userId = GetCurrentUserId();
        var activity = await _context.Activities
            .Include(a => a.Destination)
                .ThenInclude(d => d.Trip)
            .FirstOrDefaultAsync(a => a.Id == id && a.Destination.Trip.UserId == userId);

        if (activity == null)
        {
            return NotFound(new { message = "Activity not found" });
        }

        var activityDto = new ActivityDto
        {
            Id = activity.Id,
            Name = activity.Name,
            Description = activity.Description,
            ScheduledDateTime = activity.ScheduledDateTime,
            DurationMinutes = activity.DurationMinutes,
            Location = activity.Location,
            EstimatedCost = activity.EstimatedCost,
            DestinationId = activity.DestinationId,
            CreatedAt = activity.CreatedAt,
            UpdatedAt = activity.UpdatedAt
        };

        return Ok(activityDto);
    }

    [HttpPost]
    public async Task<ActionResult<ActivityDto>> CreateActivity([FromBody] CreateActivityDto createActivityDto)
    {
        var userId = GetCurrentUserId();
        var destination = await _context.Destinations
            .Include(d => d.Trip)
            .FirstOrDefaultAsync(d => d.Id == createActivityDto.DestinationId && d.Trip.UserId == userId);

        if (destination == null)
        {
            return NotFound(new { message = "Destination not found" });
        }

        if (createActivityDto.ScheduledDateTime < destination.ArrivalDate
            || createActivityDto.ScheduledDateTime > destination.DepartureDate)
        {
            return BadRequest(new { message = "Activity must be scheduled within destination dates" });
        }

        var activity = new Activity
        {
            Name = createActivityDto.Name,
            Description = createActivityDto.Description,
            ScheduledDateTime = createActivityDto.ScheduledDateTime,
            DurationMinutes = createActivityDto.DurationMinutes,
            Location = createActivityDto.Location,
            EstimatedCost = createActivityDto.EstimatedCost,
            DestinationId = createActivityDto.DestinationId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Activities.Add(activity);
        await _context.SaveChangesAsync();

        var activityDto = new ActivityDto
        {
            Id = activity.Id,
            Name = activity.Name,
            Description = activity.Description,
            ScheduledDateTime = activity.ScheduledDateTime,
            DurationMinutes = activity.DurationMinutes,
            Location = activity.Location,
            EstimatedCost = activity.EstimatedCost,
            DestinationId = activity.DestinationId,
            CreatedAt = activity.CreatedAt,
            UpdatedAt = activity.UpdatedAt
        };

        return CreatedAtAction(nameof(GetActivity), new { id = activity.Id }, activityDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ActivityDto>> UpdateActivity(int id, [FromBody] UpdateActivityDto updateActivityDto)
    {
        var userId = GetCurrentUserId();
        var activity = await _context.Activities
            .Include(a => a.Destination)
                .ThenInclude(d => d.Trip)
            .FirstOrDefaultAsync(a => a.Id == id && a.Destination.Trip.UserId == userId);

        if (activity == null)
        {
            return NotFound(new { message = "Activity not found" });
        }

        if (!string.IsNullOrEmpty(updateActivityDto.Name))
            activity.Name = updateActivityDto.Name;

        if (updateActivityDto.Description != null)
            activity.Description = updateActivityDto.Description;

        if (updateActivityDto.ScheduledDateTime.HasValue)
            activity.ScheduledDateTime = updateActivityDto.ScheduledDateTime.Value;

        if (updateActivityDto.DurationMinutes.HasValue)
            activity.DurationMinutes = updateActivityDto.DurationMinutes.Value;

        if (updateActivityDto.Location != null)
            activity.Location = updateActivityDto.Location;

        if (updateActivityDto.EstimatedCost.HasValue)
            activity.EstimatedCost = updateActivityDto.EstimatedCost;

        activity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var activityDto = new ActivityDto
        {
            Id = activity.Id,
            Name = activity.Name,
            Description = activity.Description,
            ScheduledDateTime = activity.ScheduledDateTime,
            DurationMinutes = activity.DurationMinutes,
            Location = activity.Location,
            EstimatedCost = activity.EstimatedCost,
            DestinationId = activity.DestinationId,
            CreatedAt = activity.CreatedAt,
            UpdatedAt = activity.UpdatedAt
        };

        return Ok(activityDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivity(int id)
    {
        var userId = GetCurrentUserId();
        var activity = await _context.Activities
            .Include(a => a.Destination)
                .ThenInclude(d => d.Trip)
            .FirstOrDefaultAsync(a => a.Id == id && a.Destination.Trip.UserId == userId);

        if (activity == null)
        {
            return NotFound(new { message = "Activity not found" });
        }

        _context.Activities.Remove(activity);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
