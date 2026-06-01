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
public class BudgetsController : ControllerBase
{
    private readonly TripPlannerDbContext _context;

    public BudgetsController(TripPlannerDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BudgetDto>>> GetBudgets([FromQuery] int? tripId = null)
    {
        var userId = GetCurrentUserId();
        var query = _context.Budgets
            .Include(b => b.Trip)
            .Where(b => b.Trip.UserId == userId);

        if (tripId.HasValue)
        {
            query = query.Where(b => b.TripId == tripId.Value);
        }

        var budgets = await query
            .Select(b => new BudgetDto
            {
                Id = b.Id,
                Category = b.Category,
                PlannedAmount = b.PlannedAmount,
                ActualAmount = b.ActualAmount,
                Notes = b.Notes,
                TripId = b.TripId,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .ToListAsync();

        return Ok(budgets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BudgetDto>> GetBudget(int id)
    {
        var userId = GetCurrentUserId();
        var budget = await _context.Budgets
            .Include(b => b.Trip)
            .FirstOrDefaultAsync(b => b.Id == id && b.Trip.UserId == userId);

        if (budget == null)
        {
            return NotFound(new { message = "Budget not found" });
        }

        var budgetDto = new BudgetDto
        {
            Id = budget.Id,
            Category = budget.Category,
            PlannedAmount = budget.PlannedAmount,
            ActualAmount = budget.ActualAmount,
            Notes = budget.Notes,
            TripId = budget.TripId,
            CreatedAt = budget.CreatedAt,
            UpdatedAt = budget.UpdatedAt
        };

        return Ok(budgetDto);
    }

    [HttpPost]
    public async Task<ActionResult<BudgetDto>> CreateBudget([FromBody] CreateBudgetDto createBudgetDto)
    {
        var userId = GetCurrentUserId();
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == createBudgetDto.TripId && t.UserId == userId);

        if (trip == null)
        {
            return NotFound(new { message = "Trip not found" });
        }

        var budget = new Budget
        {
            Category = createBudgetDto.Category,
            PlannedAmount = createBudgetDto.PlannedAmount,
            ActualAmount = createBudgetDto.ActualAmount,
            Notes = createBudgetDto.Notes,
            TripId = createBudgetDto.TripId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        var budgetDto = new BudgetDto
        {
            Id = budget.Id,
            Category = budget.Category,
            PlannedAmount = budget.PlannedAmount,
            ActualAmount = budget.ActualAmount,
            Notes = budget.Notes,
            TripId = budget.TripId,
            CreatedAt = budget.CreatedAt,
            UpdatedAt = budget.UpdatedAt
        };

        return CreatedAtAction(nameof(GetBudget), new { id = budget.Id }, budgetDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BudgetDto>> UpdateBudget(int id, [FromBody] UpdateBudgetDto updateBudgetDto)
    {
        var userId = GetCurrentUserId();
        var budget = await _context.Budgets
            .Include(b => b.Trip)
            .FirstOrDefaultAsync(b => b.Id == id && b.Trip.UserId == userId);

        if (budget == null)
        {
            return NotFound(new { message = "Budget not found" });
        }

        if (!string.IsNullOrEmpty(updateBudgetDto.Category))
            budget.Category = updateBudgetDto.Category;

        if (updateBudgetDto.PlannedAmount.HasValue)
            budget.PlannedAmount = updateBudgetDto.PlannedAmount.Value;

        if (updateBudgetDto.ActualAmount.HasValue)
            budget.ActualAmount = updateBudgetDto.ActualAmount.Value;

        if (updateBudgetDto.Notes != null)
            budget.Notes = updateBudgetDto.Notes;

        budget.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var budgetDto = new BudgetDto
        {
            Id = budget.Id,
            Category = budget.Category,
            PlannedAmount = budget.PlannedAmount,
            ActualAmount = budget.ActualAmount,
            Notes = budget.Notes,
            TripId = budget.TripId,
            CreatedAt = budget.CreatedAt,
            UpdatedAt = budget.UpdatedAt
        };

        return Ok(budgetDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBudget(int id)
    {
        var userId = GetCurrentUserId();
        var budget = await _context.Budgets
            .Include(b => b.Trip)
            .FirstOrDefaultAsync(b => b.Id == id && b.Trip.UserId == userId);

        if (budget == null)
        {
            return NotFound(new { message = "Budget not found" });
        }

        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
