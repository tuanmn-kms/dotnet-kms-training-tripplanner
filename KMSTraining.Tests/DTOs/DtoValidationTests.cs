using System.ComponentModel.DataAnnotations;
using KMSTraining.API.DTOs;

namespace KMSTraining.Tests.DTOs;

[TestFixture]
public class DtoValidationTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    #region CreateActivityDto Tests

    [Test]
    public void CreateActivityDto_ValidDto_PassesValidation()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            Name = "Museum Visit",
            Description = "Visit the Louvre",
            ScheduledDateTime = DateTime.UtcNow.AddDays(5),
            DurationMinutes = 180,
            Location = "Louvre Museum",
            EstimatedCost = 25.50m,
            DestinationId = 1
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void CreateActivityDto_MissingName_FailsValidation()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            Name = "",
            ScheduledDateTime = DateTime.UtcNow.AddDays(5),
            DurationMinutes = 180,
            DestinationId = 1
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Name")), Is.True);
    }

    [Test]
    public void CreateActivityDto_NameTooLong_FailsValidation()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            Name = new string('a', 201),
            ScheduledDateTime = DateTime.UtcNow.AddDays(5),
            DurationMinutes = 180,
            DestinationId = 1
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Name")), Is.True);
    }

    [Test]
    public void CreateActivityDto_NegativeDuration_FailsValidation()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            Name = "Test Activity",
            ScheduledDateTime = DateTime.UtcNow.AddDays(5),
            DurationMinutes = -10,
            DestinationId = 1
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("DurationMinutes")), Is.True);
    }

    #endregion

    #region UpdateActivityDto Tests

    [Test]
    public void UpdateActivityDto_ValidDto_PassesValidation()
    {
        // Arrange
        var dto = new UpdateActivityDto
        {
            Name = "Updated Activity",
            Description = "Updated description",
            DurationMinutes = 120
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void UpdateActivityDto_NameTooLong_FailsValidation()
    {
        // Arrange
        var dto = new UpdateActivityDto
        {
            Name = new string('a', 201)
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Name")), Is.True);
    }

    [Test]
    public void UpdateActivityDto_NegativeDuration_FailsValidation()
    {
        // Arrange
        var dto = new UpdateActivityDto
        {
            DurationMinutes = -5
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("DurationMinutes")), Is.True);
    }

    #endregion

    #region CreateBudgetDto Tests

    [Test]
    public void CreateBudgetDto_ValidDto_PassesValidation()
    {
        // Arrange
        var dto = new CreateBudgetDto
        {
            Category = "Accommodation",
            PlannedAmount = 1000m,
            ActualAmount = 950m,
            Notes = "Hotel expenses",
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void CreateBudgetDto_MissingCategory_FailsValidation()
    {
        // Arrange
        var dto = new CreateBudgetDto
        {
            Category = "",
            PlannedAmount = 1000m,
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Category")), Is.True);
    }

    [Test]
    public void CreateBudgetDto_NegativePlannedAmount_FailsValidation()
    {
        // Arrange
        var dto = new CreateBudgetDto
        {
            Category = "Accommodation",
            PlannedAmount = -100m,
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("PlannedAmount")), Is.True);
    }

    [Test]
    public void CreateBudgetDto_NegativeActualAmount_FailsValidation()
    {
        // Arrange
        var dto = new CreateBudgetDto
        {
            Category = "Accommodation",
            PlannedAmount = 1000m,
            ActualAmount = -50m,
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("ActualAmount")), Is.True);
    }

    #endregion

    #region UpdateBudgetDto Tests

    [Test]
    public void UpdateBudgetDto_ValidDto_PassesValidation()
    {
        // Arrange
        var dto = new UpdateBudgetDto
        {
            Category = "Transportation",
            PlannedAmount = 500m,
            ActualAmount = 475m
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void UpdateBudgetDto_NegativePlannedAmount_FailsValidation()
    {
        // Arrange
        var dto = new UpdateBudgetDto
        {
            PlannedAmount = -100m
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("PlannedAmount")), Is.True);
    }

    #endregion

    #region CreateTripDto Tests

    [Test]
    public void CreateTripDto_ValidDto_PassesValidation()
    {
        // Arrange
        var dto = new CreateTripDto
        {
            Name = "Summer Vacation",
            Description = "Trip to Europe",
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(45)
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void CreateTripDto_MissingName_FailsValidation()
    {
        // Arrange
        var dto = new CreateTripDto
        {
            Name = "",
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(45)
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Name")), Is.True);
    }

    [Test]
    public void CreateTripDto_NameTooLong_FailsValidation()
    {
        // Arrange
        var dto = new CreateTripDto
        {
            Name = new string('a', 201),
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(45)
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Name")), Is.True);
    }

    #endregion

    #region UpdateTripDto Tests

    [Test]
    public void UpdateTripDto_ValidDto_PassesValidation()
    {
        // Arrange
        var dto = new UpdateTripDto
        {
            Name = "Updated Trip Name",
            Description = "Updated description",
            Status = "Confirmed"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void UpdateTripDto_NameTooLong_FailsValidation()
    {
        // Arrange
        var dto = new UpdateTripDto
        {
            Name = new string('a', 201)
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Name")), Is.True);
    }

    [Test]
    public void UpdateTripDto_DescriptionTooLong_FailsValidation()
    {
        // Arrange
        var dto = new UpdateTripDto
        {
            Description = new string('a', 1001)
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Description")), Is.True);
    }

    [Test]
    public void UpdateTripDto_StatusTooLong_FailsValidation()
    {
        // Arrange
        var dto = new UpdateTripDto
        {
            Status = new string('a', 51)
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Status")), Is.True);
    }

    #endregion

    #region CreateDestinationDto Tests

    [Test]
    public void CreateDestinationDto_ValidDto_PassesValidation()
    {
        // Arrange
        var dto = new CreateDestinationDto
        {
            Name = "Paris",
            Country = "France",
            City = "Paris",
            Description = "The City of Light",
            ArrivalDate = DateTime.UtcNow.AddDays(5),
            DepartureDate = DateTime.UtcNow.AddDays(8),
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void CreateDestinationDto_MissingName_FailsValidation()
    {
        // Arrange
        var dto = new CreateDestinationDto
        {
            Name = "",
            Country = "France",
            ArrivalDate = DateTime.UtcNow.AddDays(5),
            DepartureDate = DateTime.UtcNow.AddDays(8),
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Name")), Is.True);
    }

    [Test]
    public void CreateDestinationDto_MissingCountry_FailsValidation()
    {
        // Arrange
        var dto = new CreateDestinationDto
        {
            Name = "Paris",
            Country = "",
            ArrivalDate = DateTime.UtcNow.AddDays(5),
            DepartureDate = DateTime.UtcNow.AddDays(8),
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Country")), Is.True);
    }

    [Test]
    public void CreateDestinationDto_NameTooLong_FailsValidation()
    {
        // Arrange
        var dto = new CreateDestinationDto
        {
            Name = new string('a', 201),
            Country = "France",
            ArrivalDate = DateTime.UtcNow.AddDays(5),
            DepartureDate = DateTime.UtcNow.AddDays(8),
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Name")), Is.True);
    }

    #endregion

    #region UpdateDestinationDto Tests

    [Test]
    public void UpdateDestinationDto_ValidDto_PassesValidation()
    {
        // Arrange
        var dto = new UpdateDestinationDto
        {
            Name = "Updated Paris",
            Country = "France",
            City = "Paris"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void UpdateDestinationDto_NameTooLong_FailsValidation()
    {
        // Arrange
        var dto = new UpdateDestinationDto
        {
            Name = new string('a', 201)
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Name")), Is.True);
    }

    #endregion

    #region AuthDto Tests

    [Test]
    public void LoginDto_ValidDto_PassesValidation()
    {
        // Arrange
        var dto = new LoginDto
        {
            UsernameOrEmail = "testuser",
            Password = "password123"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void LoginDto_MissingUsernameOrEmail_FailsValidation()
    {
        // Arrange
        var dto = new LoginDto
        {
            UsernameOrEmail = "",
            Password = "password123"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("UsernameOrEmail")), Is.True);
    }

    [Test]
    public void LoginDto_MissingPassword_FailsValidation()
    {
        // Arrange
        var dto = new LoginDto
        {
            UsernameOrEmail = "testuser",
            Password = ""
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Password")), Is.True);
    }

    [Test]
    public void RegisterDto_ValidDto_PassesValidation()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "password123",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void RegisterDto_MissingUsername_FailsValidation()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Username = "",
            Email = "newuser@example.com",
            Password = "password123"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Username")), Is.True);
    }

    [Test]
    public void RegisterDto_InvalidEmail_FailsValidation()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Username = "newuser",
            Email = "invalid-email",
            Password = "password123"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Email")), Is.True);
    }

    [Test]
    public void RegisterDto_PasswordTooShort_FailsValidation()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "12345" // Less than 6 characters
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Password")), Is.True);
    }

    #endregion
}
