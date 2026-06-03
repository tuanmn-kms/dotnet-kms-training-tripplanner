using System.ComponentModel.DataAnnotations;

namespace KMSTraining.API.Domain.Entities;

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

    public int TripId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Trip Trip { get; set; } = null!;
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public static Destination Create(
        string name,
        string country,
        DateTime arrivalDate,
        DateTime departureDate,
        int tripId,
        string? city = null,
        string? description = null)
    {
        return new Destination
        {
            Name = name,
            Country = country,
            ArrivalDate = arrivalDate,
            DepartureDate = departureDate,
            TripId = tripId,
            City = city,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string? name = null,
        string? country = null,
        DateTime? arrivalDate = null,
        DateTime? departureDate = null,
        string? city = null,
        string? description = null)
    {
        if (name != null)
        {
            Name = name;
        }

        if (country != null)
        {
            Country = country;
        }

        if (arrivalDate.HasValue)
        {
            ArrivalDate = arrivalDate.Value;
        }

        if (departureDate.HasValue)
        {
            DepartureDate = departureDate.Value;
        }

        if (city != null)
        {
            City = city;
        }

        if (description != null)
        {
            Description = description;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
