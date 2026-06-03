using System.ComponentModel.DataAnnotations;

namespace KMSTraining.API.Application.DTOs;

public class CreateTripDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
}

public class UpdateTripDto
{
    [StringLength(200)]
    public string? Name { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }
}

public class TripDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TripDetailDto : TripDto
{
    public List<DestinationDto> Destinations { get; set; } = new();
    public List<BudgetDto> Budgets { get; set; } = new();
}
