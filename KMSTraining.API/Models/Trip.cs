using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSTraining.API.Models;

public class Trip
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = TripStatus.Planning;

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<Destination> Destinations { get; set; } = new List<Destination>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
}

public static class TripStatus
{
    public const string Planning = "Planning";
    public const string Confirmed = "Confirmed";
    public const string InProgress = "InProgress";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";
}
