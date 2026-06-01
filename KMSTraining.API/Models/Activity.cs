using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSTraining.API.Models;

public class Activity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DateTime ScheduledDateTime { get; set; }

    [Range(0, int.MaxValue)]
    public int DurationMinutes { get; set; }

    [StringLength(200)]
    public string? Location { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? EstimatedCost { get; set; }

    [ForeignKey(nameof(Destination))]
    public int DestinationId { get; set; }

    public Destination Destination { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
