using KMSTraining.API.Application.DTOs;
using KMSTraining.API.Application.Services;
using KMSTraining.API.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMSTraining.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DestinationsController : ControllerBase
{
    private readonly IDestinationService _destinationService;
    private readonly ILogger<DestinationsController> _logger;

    public DestinationsController(IDestinationService destinationService, ILogger<DestinationsController> logger)
    {
        _destinationService = destinationService;
        _logger = logger;
    }


    /// <summary>
    /// Get all destinations for a trip
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DestinationDto>>> GetDestinations([FromQuery] int? tripId = null)
    {
        try
        {
            _logger.LogInformation("Fetching destinations for trip {TripId}", tripId);
            var destinations = await _destinationService.GetTripDestinationsAsync(tripId ?? 0);
            return Ok(destinations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching destinations for trip {TripId}", tripId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error fetching destinations" });
        }
    }

    /// <summary>
    /// Get a specific destination
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DestinationDetailDto>> GetDestination(int id)
    {
        try
        {
            _logger.LogInformation("Fetching destination {DestinationId}", id);
            var destination = await _destinationService.GetDestinationByIdAsync(id);

            if (destination == null)
            {
                return NotFound(new { message = "Destination not found" });
            }
            
            return Ok(destination);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching destination {DestinationId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error fetching destination" });
        }
    }

    /// <summary>
    /// Create a new destination
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DestinationDto>> CreateDestination([FromBody] CreateDestinationDto createDestinationDto)
    {
        try
        {
            _logger.LogInformation("Creating destination for trip {TripId}", createDestinationDto.TripId);
            var destination = await _destinationService.CreateDestinationAsync(createDestinationDto.TripId, createDestinationDto);
            return CreatedAtAction(nameof(GetDestination), new { id = destination.Id }, destination);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating destination");
            return BadRequest(new { message = "Error creating destination" });
        }
    }

    /// <summary>
    /// Update a destination
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DestinationDto>> UpdateDestination(int id, [FromBody] UpdateDestinationDto updateDestinationDto)
    {
        try
        {
            _logger.LogInformation("Updating destination {DestinationId}", id);
            var destination = await _destinationService.UpdateDestinationAsync(id, updateDestinationDto);
            return Ok(destination);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning("Destination not found: {DestinationId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating destination {DestinationId}", id);
            return BadRequest(new { message = "Error updating destination" });
        }
    }

    /// <summary>
    /// Delete a destination
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDestination(int id)
    {
        try
        {
            _logger.LogInformation("Deleting destination {DestinationId}", id);
            var result = await _destinationService.DeleteDestinationAsync(id);
            
            if (!result)
            {
                return NotFound(new { message = "Destination not found" });
            }
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting destination {DestinationId}", id);
            return BadRequest(new { message = "Error deleting destination" });
        }
    }
}
