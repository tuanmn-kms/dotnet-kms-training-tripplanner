using System.ComponentModel.DataAnnotations;

namespace KMSTraining.API.Domain.Entities;

public class Budget
{
    [Key]
    public int Id { get; set; }

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

    public int TripId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Trip Trip { get; set; } = null!;

    public static Budget Create(string category, decimal plannedAmount, int tripId, decimal actualAmount = 0, string? notes = null)
    {
        return new Budget
        {
            Category = category,
            PlannedAmount = plannedAmount,
            ActualAmount = actualAmount,
            Notes = notes,
            TripId = tripId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string? category = null, decimal? plannedAmount = null, decimal? actualAmount = null, string? notes = null)
    {
        if (category != null)
        {
            Category = category;
        }

        if (plannedAmount.HasValue)
        {
            PlannedAmount = plannedAmount.Value;
        }

        if (actualAmount.HasValue)
        {
            ActualAmount = actualAmount.Value;
        }

        if (notes != null)
        {
            Notes = notes;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordExpense(decimal amount)
    {
        ActualAmount += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal GetRemaining() => PlannedAmount - ActualAmount;
}
