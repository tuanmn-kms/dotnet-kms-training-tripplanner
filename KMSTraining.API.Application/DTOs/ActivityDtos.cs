using System.ComponentModel.DataAnnotations;

namespace KMSTraining.API.Application.DTOs;

public class CreateActivityDto
{
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

    [Required]
    public int DestinationId { get; set; }
}

public class UpdateActivityDto
{
    [StringLength(200)]
    public string? Name { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime? ScheduledDateTime { get; set; }

    [Range(0, int.MaxValue)]
    public int? DurationMinutes { get; set; }

    [StringLength(200)]
    public string? Location { get; set; }

    public decimal? EstimatedCost { get; set; }
}

public class ActivityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public int DurationMinutes { get; set; }
    public string? Location { get; set; }
    public decimal? EstimatedCost { get; set; }
    public int DestinationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
