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
public class DestinationsController : ControllerBase
{
    private readonly TripPlannerDbContext _context;

    public DestinationsController(TripPlannerDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DestinationDto>>> GetDestinations([FromQuery] int? tripId = null)
    {
        var userId = GetCurrentUserId();
        var query = _context.Destinations
            .Include(d => d.Trip)
            .Where(d => d.Trip.UserId == userId);

        if (tripId.HasValue)
        {
            query = query.Where(d => d.TripId == tripId.Value);
        }

        var destinations = await query
            .Select(d => new DestinationDto
            {
                Id = d.Id,
                Name = d.Name,
                Country = d.Country,
                City = d.City,
                Description = d.Description,
                ArrivalDate = d.ArrivalDate,
                DepartureDate = d.DepartureDate,
                TripId = d.TripId,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .ToListAsync();

        return Ok(destinations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DestinationDetailDto>> GetDestination(int id)
    {
        var userId = GetCurrentUserId();
        var destination = await _context.Destinations
            .Include(d => d.Trip)
            .Include(d => d.Activities)
            .FirstOrDefaultAsync(d => d.Id == id && d.Trip.UserId == userId);

        if (destination == null)
        {
            return NotFound(new { message = "Destination not found" });
        }

        var destinationDto = new DestinationDetailDto
        {
            Id = destination.Id,
            Name = destination.Name,
            Country = destination.Country,
            City = destination.City,
            Description = destination.Description,
            ArrivalDate = destination.ArrivalDate,
            DepartureDate = destination.DepartureDate,
            TripId = destination.TripId,
            CreatedAt = destination.CreatedAt,
            UpdatedAt = destination.UpdatedAt,
            Activities = destination.Activities.Select(a => new ActivityDto
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
            }).ToList()
        };

        return Ok(destinationDto);
    }

    [HttpPost]
    public async Task<ActionResult<DestinationDto>> CreateDestination([FromBody] CreateDestinationDto createDestinationDto)
    {
        var userId = GetCurrentUserId();
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == createDestinationDto.TripId && t.UserId == userId);

        if (trip == null)
        {
            return NotFound(new { message = "Trip not found" });
        }

        if (createDestinationDto.DepartureDate <= createDestinationDto.ArrivalDate)
        {
            return BadRequest(new { message = "Departure date must be after arrival date" });
        }

        if (createDestinationDto.ArrivalDate < trip.StartDate || createDestinationDto.DepartureDate > trip.EndDate)
        {
            return BadRequest(new { message = "Destination dates must be within trip dates" });
        }

        var destination = new Destination
        {
            Name = createDestinationDto.Name,
            Country = createDestinationDto.Country,
            City = createDestinationDto.City,
            Description = createDestinationDto.Description,
            ArrivalDate = createDestinationDto.ArrivalDate,
            DepartureDate = createDestinationDto.DepartureDate,
            TripId = createDestinationDto.TripId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Destinations.Add(destination);
        await _context.SaveChangesAsync();

        var destinationDto = new DestinationDto
        {
            Id = destination.Id,
            Name = destination.Name,
            Country = destination.Country,
            City = destination.City,
            Description = destination.Description,
            ArrivalDate = destination.ArrivalDate,
            DepartureDate = destination.DepartureDate,
            TripId = destination.TripId,
            CreatedAt = destination.CreatedAt,
            UpdatedAt = destination.UpdatedAt
        };

        return CreatedAtAction(nameof(GetDestination), new { id = destination.Id }, destinationDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DestinationDto>> UpdateDestination(int id, [FromBody] UpdateDestinationDto updateDestinationDto)
    {
        var userId = GetCurrentUserId();
        var destination = await _context.Destinations
            .Include(d => d.Trip)
            .FirstOrDefaultAsync(d => d.Id == id && d.Trip.UserId == userId);

        if (destination == null)
        {
            return NotFound(new { message = "Destination not found" });
        }

        if (updateDestinationDto.ArrivalDate.HasValue && updateDestinationDto.DepartureDate.HasValue &&
            updateDestinationDto.DepartureDate.Value <= updateDestinationDto.ArrivalDate.Value)
        {
            return BadRequest(new { message = "Departure date must be after arrival date" });
        }

        if (!string.IsNullOrEmpty(updateDestinationDto.Name))
            destination.Name = updateDestinationDto.Name;

        if (!string.IsNullOrEmpty(updateDestinationDto.Country))
            destination.Country = updateDestinationDto.Country;

        if (updateDestinationDto.City != null)
            destination.City = updateDestinationDto.City;

        if (updateDestinationDto.Description != null)
            destination.Description = updateDestinationDto.Description;

        if (updateDestinationDto.ArrivalDate.HasValue)
            destination.ArrivalDate = updateDestinationDto.ArrivalDate.Value;

        if (updateDestinationDto.DepartureDate.HasValue)
            destination.DepartureDate = updateDestinationDto.DepartureDate.Value;

        destination.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var destinationDto = new DestinationDto
        {
            Id = destination.Id,
            Name = destination.Name,
            Country = destination.Country,
            City = destination.City,
            Description = destination.Description,
            ArrivalDate = destination.ArrivalDate,
            DepartureDate = destination.DepartureDate,
            TripId = destination.TripId,
            CreatedAt = destination.CreatedAt,
            UpdatedAt = destination.UpdatedAt
        };

        return Ok(destinationDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDestination(int id)
    {
        var userId = GetCurrentUserId();
        var destination = await _context.Destinations
            .Include(d => d.Trip)
            .FirstOrDefaultAsync(d => d.Id == id && d.Trip.UserId == userId);

        if (destination == null)
        {
            return NotFound(new { message = "Destination not found" });
        }

        _context.Destinations.Remove(destination);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
