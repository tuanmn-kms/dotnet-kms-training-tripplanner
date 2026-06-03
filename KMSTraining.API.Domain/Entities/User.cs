using System.ComponentModel.DataAnnotations;

namespace KMSTraining.API.Domain.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Trip> Trips { get; set; } = new List<Trip>();

    public static User Create(string username, string email, string passwordHash, string? firstName = null, string? lastName = null)
    {
        return new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string? firstName = null, string? lastName = null)
    {
        if (firstName != null)
        {
            FirstName = firstName;
        }

        if (lastName != null)
        {
            LastName = lastName;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
