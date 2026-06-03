using System.ComponentModel.DataAnnotations;

namespace KMSTraining.API.Application.DTOs;

public class CreateBudgetDto
{
    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal PlannedAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ActualAmount { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [Required]
    public int TripId { get; set; }
}

public class UpdateBudgetDto
{
    [StringLength(100)]
    public string? Category { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? PlannedAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? ActualAmount { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class BudgetDto
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal PlannedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public string? Notes { get; set; }
    public int TripId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
