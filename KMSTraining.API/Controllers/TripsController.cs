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
public class TripsController : ControllerBase
{
    private readonly TripPlannerDbContext _context;

    public TripsController(TripPlannerDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TripDto>>> GetTrips()
    {
        var userId = GetCurrentUserId();
        var trips = await _context.Trips
            .Where(t => t.UserId == userId)
            .Select(t => new TripDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Status = t.Status,
                UserId = t.UserId,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();

        return Ok(trips);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TripDetailDto>> GetTrip(int id)
    {
        var userId = GetCurrentUserId();
        var trip = await _context.Trips
            .Include(t => t.Destinations)
                .ThenInclude(d => d.Activities)
            .Include(t => t.Budgets)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (trip == null)
        {
            return NotFound(new { message = "Trip not found" });
        }

        var tripDto = new TripDetailDto
        {
            Id = trip.Id,
            Name = trip.Name,
            Description = trip.Description,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            Status = trip.Status,
            UserId = trip.UserId,
            CreatedAt = trip.CreatedAt,
            UpdatedAt = trip.UpdatedAt,
            Destinations = trip.Destinations.Select(d => new DestinationDto
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
            }).ToList(),
            Budgets = trip.Budgets.Select(b => new BudgetDto
            {
                Id = b.Id,
                Category = b.Category,
                PlannedAmount = b.PlannedAmount,
                ActualAmount = b.ActualAmount,
                Notes = b.Notes,
                TripId = b.TripId,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            }).ToList()
        };

        return Ok(tripDto);
    }

    [HttpPost]
    public async Task<ActionResult<TripDto>> CreateTrip([FromBody] CreateTripDto createTripDto)
    {
        var userId = GetCurrentUserId();

        if (createTripDto.EndDate <= createTripDto.StartDate)
        {
            return BadRequest(new { message = "End date must be after start date" });
        }

        var trip = new Trip
        {
            Name = createTripDto.Name,
            Description = createTripDto.Description,
            StartDate = createTripDto.StartDate,
            EndDate = createTripDto.EndDate,
            Status = TripStatus.Planning,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        var tripDto = new TripDto
        {
            Id = trip.Id,
            Name = trip.Name,
            Description = trip.Description,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            Status = trip.Status,
            UserId = trip.UserId,
            CreatedAt = trip.CreatedAt,
            UpdatedAt = trip.UpdatedAt
        };

        return CreatedAtAction(nameof(GetTrip), new { id = trip.Id }, tripDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TripDto>> UpdateTrip(int id, [FromBody] UpdateTripDto updateTripDto)
    {
        var userId = GetCurrentUserId();
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (trip == null)
        {
            return NotFound(new { message = "Trip not found" });
        }

        if (updateTripDto.StartDate.HasValue && updateTripDto.EndDate.HasValue &&
            updateTripDto.EndDate.Value <= updateTripDto.StartDate.Value)
        {
            return BadRequest(new { message = "End date must be after start date" });
        }

        if (!string.IsNullOrEmpty(updateTripDto.Name))
            trip.Name = updateTripDto.Name;

        if (updateTripDto.Description != null)
            trip.Description = updateTripDto.Description;

        if (updateTripDto.StartDate.HasValue)
            trip.StartDate = updateTripDto.StartDate.Value;

        if (updateTripDto.EndDate.HasValue)
            trip.EndDate = updateTripDto.EndDate.Value;

        if (!string.IsNullOrEmpty(updateTripDto.Status))
        {
            var validStatuses = new[] { TripStatus.Planning, TripStatus.Confirmed, TripStatus.InProgress, TripStatus.Completed, TripStatus.Cancelled };
            if (validStatuses.Contains(updateTripDto.Status))
            {
                trip.Status = updateTripDto.Status;
            }
            else
            {
                return BadRequest(new { message = "Invalid status value" });
            }
        }

        trip.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var tripDto = new TripDto
        {
            Id = trip.Id,
            Name = trip.Name,
            Description = trip.Description,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            Status = trip.Status,
            UserId = trip.UserId,
            CreatedAt = trip.CreatedAt,
            UpdatedAt = trip.UpdatedAt
        };

        return Ok(tripDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTrip(int id)
    {
        var userId = GetCurrentUserId();
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (trip == null)
        {
            return NotFound(new { message = "Trip not found" });
        }

        _context.Trips.Remove(trip);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
