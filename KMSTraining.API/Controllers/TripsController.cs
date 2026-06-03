using KMSTraining.API.Application.DTOs;
using KMSTraining.API.Application.Services;
using KMSTraining.API.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMSTraining.API.Controllers;

public class ChangeStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;
    private readonly ILogger<TripsController> _logger;

    public TripsController(ITripService tripService, ILogger<TripsController> logger)
    {
        _tripService = tripService;
        _logger = logger;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    /// <summary>
    /// Get all trips for the current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TripDto>>> GetTrips()
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Fetching trips for user {UserId}", userId);
            var trips = await _tripService.GetUserTripsAsync(userId);
            return Ok(trips);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching trips");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error fetching trips" });
        }
    }

    /// <summary>
    /// Get a specific trip with all details
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripDetailDto>> GetTrip(int id)
    {
        try
        {
            _logger.LogInformation("Fetching trip {TripId}", id);
            var trip = await _tripService.GetTripByIdAsync(id);

            if (trip == null)
            {
                return NotFound(new { message = "Trip not found" });
            }
            
            return Ok(trip);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching trip {TripId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error fetching trip" });
        }
    }

    /// <summary>
    /// Create a new trip
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TripDto>> CreateTrip([FromBody] CreateTripDto createTripDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Creating new trip for user {UserId}", userId);
            
            var trip = await _tripService.CreateTripAsync(userId, createTripDto);
            return CreatedAtAction(nameof(GetTrip), new { id = trip.Id }, trip);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating trip");
            return BadRequest(new { message = "Error creating trip" });
        }
    }

    /// <summary>
    /// Update an existing trip
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripDto>> UpdateTrip(int id, [FromBody] UpdateTripDto updateTripDto)
    {
        try
        {
            _logger.LogInformation("Updating trip {TripId}", id);
            var trip = await _tripService.UpdateTripAsync(id, updateTripDto);
            return Ok(trip);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning("Trip not found: {TripId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating trip {TripId}", id);
            return BadRequest(new { message = "Error updating trip" });
        }
    }

    /// <summary>
    /// Delete a trip
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTrip(int id)
    {
        try
        {
            _logger.LogInformation("Deleting trip {TripId}", id);
            var result = await _tripService.DeleteTripAsync(id);
            
            if (!result)
            {
                return NotFound(new { message = "Trip not found" });
            }
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting trip {TripId}", id);
            return BadRequest(new { message = "Error deleting trip" });
        }
    }

    /// <summary>
    /// Change trip status
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripDto>> ChangeTripStatus(int id, [FromBody] ChangeStatusRequest request)
    {
        try
        {
            _logger.LogInformation("Changing trip {TripId} status to {Status}", id, request.Status);
            await _tripService.ChangeTripStatusAsync(id, request.Status);
            var trip = await _tripService.GetTripByIdAsync(id);
            return Ok(trip);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning("Trip not found: {TripId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing trip status {TripId}", id);
            return BadRequest(new { message = "Error changing trip status" });
        }
    }
}
