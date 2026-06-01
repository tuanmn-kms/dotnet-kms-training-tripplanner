using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSTraining.API.Models;

public class Destination
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DateTime ArrivalDate { get; set; }

    [Required]
    public DateTime DepartureDate { get; set; }

    [ForeignKey(nameof(Trip))]
    public int TripId { get; set; }

    public Trip Trip { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
