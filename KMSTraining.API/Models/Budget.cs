using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSTraining.API.Models;

public class Budget
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal PlannedAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal ActualAmount { get; set; } = 0;

    [StringLength(500)]
    public string? Notes { get; set; }

    [ForeignKey(nameof(Trip))]
    public int TripId { get; set; }

    public Trip Trip { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
