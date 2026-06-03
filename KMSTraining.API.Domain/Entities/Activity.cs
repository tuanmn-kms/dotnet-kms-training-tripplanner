using System.ComponentModel.DataAnnotations;

namespace KMSTraining.API.Domain.Entities;

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

    public decimal? EstimatedCost { get; set; }

    public int DestinationId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Destination Destination { get; set; } = null!;

    public static Activity Create(
        string name,
        DateTime scheduledDateTime,
        int destinationId,
        string? description = null,
        int durationMinutes = 0,
        string? location = null,
        decimal? estimatedCost = null)
    {
        return new Activity
        {
            Name = name,
            ScheduledDateTime = scheduledDateTime,
            DurationMinutes = durationMinutes,
            Location = location,
            EstimatedCost = estimatedCost,
            DestinationId = destinationId,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string? name = null,
        DateTime? scheduledDateTime = null,
        string? description = null,
        int? durationMinutes = null,
        string? location = null,
        decimal? estimatedCost = null)
    {
        if (name != null)
        {
            Name = name;
        }

        if (scheduledDateTime.HasValue)
        {
            ScheduledDateTime = scheduledDateTime.Value;
        }

        if (description != null)
        {
            Description = description;
        }

        if (durationMinutes.HasValue)
        {
            DurationMinutes = durationMinutes.Value;
        }

        if (location != null)
        {
            Location = location;
        }

        if (estimatedCost.HasValue)
        {
            EstimatedCost = estimatedCost.Value;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
