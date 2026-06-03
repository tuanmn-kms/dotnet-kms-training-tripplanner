using System.ComponentModel.DataAnnotations;

namespace KMSTraining.API.Domain.Entities;

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

    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Destination> Destinations { get; set; } = new List<Destination>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();

    public static Trip Create(string name, int userId, DateTime startDate, DateTime endDate, string? description = null)
    {
        return new Trip
        {
            Name = name,
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate,
            Description = description,
            Status = TripStatus.Planning,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, DateTime startDate, DateTime endDate, string? description = null)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(string newStatus)
    {
        if (!IsValidStatus(newStatus))
        {
            throw new InvalidOperationException($"Invalid trip status: {newStatus}");
        }

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    private static bool IsValidStatus(string status) =>
        status switch
        {
            TripStatus.Planning or TripStatus.Confirmed or TripStatus.InProgress or 
            TripStatus.Completed or TripStatus.Cancelled => true,
            _ => false
        };
}

public static class TripStatus
{
    public const string Planning = "Planning";
    public const string Confirmed = "Confirmed";
    public const string InProgress = "InProgress";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";
}
