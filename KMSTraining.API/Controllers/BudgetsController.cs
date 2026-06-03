using KMSTraining.API.Application.DTOs;
using KMSTraining.API.Application.Services;
using KMSTraining.API.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMSTraining.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _budgetService;
    private readonly ILogger<BudgetsController> _logger;

    public BudgetsController(IBudgetService budgetService, ILogger<BudgetsController> logger)
    {
        _budgetService = budgetService;
        _logger = logger;
    }


    /// <summary>
    /// Get all budgets for a trip
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BudgetDto>>> GetBudgets([FromQuery] int? tripId = null)
    {
        try
        {
            _logger.LogInformation("Fetching budgets for trip {TripId}", tripId);
            var budgets = await _budgetService.GetTripBudgetsAsync(tripId ?? 0);
            return Ok(budgets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching budgets for trip {TripId}", tripId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error fetching budgets" });
        }
    }

    /// <summary>
    /// Get a specific budget
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BudgetDto>> GetBudget(int id)
    {
        try
        {
            _logger.LogInformation("Fetching budget {BudgetId}", id);
            var budget = await _budgetService.GetBudgetByIdAsync(id);

            if (budget == null)
            {
                return NotFound(new { message = "Budget not found" });
            }
            
            return Ok(budget);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching budget {BudgetId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error fetching budget" });
        }
    }

    /// <summary>
    /// Create a new budget
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BudgetDto>> CreateBudget([FromBody] CreateBudgetDto createBudgetDto)
    {
        try
        {
            _logger.LogInformation("Creating budget for trip {TripId}", createBudgetDto.TripId);
            var budget = await _budgetService.CreateBudgetAsync(createBudgetDto.TripId, createBudgetDto);
            return CreatedAtAction(nameof(GetBudget), new { id = budget.Id }, budget);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating budget");
            return BadRequest(new { message = "Error creating budget" });
        }
    }

    /// <summary>
    /// Update a budget
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BudgetDto>> UpdateBudget(int id, [FromBody] UpdateBudgetDto updateBudgetDto)
    {
        try
        {
            _logger.LogInformation("Updating budget {BudgetId}", id);
            var budget = await _budgetService.UpdateBudgetAsync(id, updateBudgetDto);
            return Ok(budget);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning("Budget not found: {BudgetId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating budget {BudgetId}", id);
            return BadRequest(new { message = "Error updating budget" });
        }
    }

    /// <summary>
    /// Delete a budget
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBudget(int id)
    {
        try
        {
            _logger.LogInformation("Deleting budget {BudgetId}", id);
            var result = await _budgetService.DeleteBudgetAsync(id);
            
            if (!result)
            {
                return NotFound(new { message = "Budget not found" });
            }
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting budget {BudgetId}", id);
            return BadRequest(new { message = "Error deleting budget" });
        }
    }
}
