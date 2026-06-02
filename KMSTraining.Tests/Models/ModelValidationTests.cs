using System.ComponentModel.DataAnnotations;
using KMSTraining.API.Models;

namespace KMSTraining.Tests.Models;

[TestFixture]
public class ModelValidationTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    #region User Model Tests

    [Test]
    public void User_ValidModel_PassesValidation()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword123"
        };

        // Act
        var validationResults = ValidateModel(user);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void User_MissingUsername_FailsValidation()
    {
        // Arrange
        var user = new User
        {
            Username = "",
            Email = "test@example.com",
            PasswordHash = "hashedpassword123"
        };

        // Act
        var validationResults = ValidateModel(user);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Username")), Is.True);
    }

    [Test]
    public void User_InvalidEmail_FailsValidation()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "invalid-email",
            PasswordHash = "hashedpassword123"
        };

        // Act
        var validationResults = ValidateModel(user);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Email")), Is.True);
    }

    [Test]
    public void User_UsernameTooLong_FailsValidation()
    {
        // Arrange
        var user = new User
        {
            Username = new string('a', 101), // 101 characters (max is 100)
            Email = "test@example.com",
            PasswordHash = "hashedpassword123"
        };

        // Act
        var validationResults = ValidateModel(user);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Username")), Is.True);
    }

    [Test]
    public void User_EmailTooLong_FailsValidation()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = new string('a', 90) + "@example.com", // More than 100 characters
            PasswordHash = "hashedpassword123"
        };

        // Act
        var validationResults = ValidateModel(user);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Email")), Is.True);
    }

    #endregion

    #region Trip Model Tests

    [Test]
    public void Trip_ValidModel_PassesValidation()
    {
        // Arrange
        var trip = new Trip
        {
            Name = "Summer Vacation",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(10),
            Status = TripStatus.Planning,
            UserId = 1
        };

        // Act
        var validationResults = ValidateModel(trip);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void Trip_MissingName_FailsValidation()
    {
        // Arrange
        var trip = new Trip
        {
            Name = "",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(10),
            UserId = 1
        };

        // Act
        var validationResults = ValidateModel(trip);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Name")), Is.True);
    }

    [Test]
    public void Trip_NameTooLong_FailsValidation()
    {
        // Arrange
        var trip = new Trip
        {
            Name = new string('a', 201), // 201 characters (max is 200)
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(10),
            UserId = 1
        };

        // Act
        var validationResults = ValidateModel(trip);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Name")), Is.True);
    }

    [Test]
    public void Trip_DescriptionTooLong_FailsValidation()
    {
        // Arrange
        var trip = new Trip
        {
            Name = "Test Trip",
            Description = new string('a', 1001), // 1001 characters (max is 1000)
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(10),
            UserId = 1
        };

        // Act
        var validationResults = ValidateModel(trip);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Description")), Is.True);
    }

    #endregion

    #region Destination Model Tests

    [Test]
    public void Destination_ValidModel_PassesValidation()
    {
        // Arrange
        var destination = new Destination
        {
            Name = "Paris",
            Country = "France",
            City = "Paris",
            ArrivalDate = DateTime.UtcNow.AddDays(5),
            DepartureDate = DateTime.UtcNow.AddDays(8),
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(destination);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void Destination_MissingName_FailsValidation()
    {
        // Arrange
        var destination = new Destination
        {
            Name = "",
            Country = "France",
            ArrivalDate = DateTime.UtcNow.AddDays(5),
            DepartureDate = DateTime.UtcNow.AddDays(8),
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(destination);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Name")), Is.True);
    }

    [Test]
    public void Destination_MissingCountry_FailsValidation()
    {
        // Arrange
        var destination = new Destination
        {
            Name = "Paris",
            Country = "",
            ArrivalDate = DateTime.UtcNow.AddDays(5),
            DepartureDate = DateTime.UtcNow.AddDays(8),
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(destination);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Country")), Is.True);
    }

    #endregion

    #region Activity Model Tests

    [Test]
    public void Activity_ValidModel_PassesValidation()
    {
        // Arrange
        var activity = new Activity
        {
            Name = "Eiffel Tower Visit",
            ScheduledDateTime = DateTime.UtcNow.AddDays(6),
            DurationMinutes = 120,
            DestinationId = 1
        };

        // Act
        var validationResults = ValidateModel(activity);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void Activity_MissingName_FailsValidation()
    {
        // Arrange
        var activity = new Activity
        {
            Name = "",
            ScheduledDateTime = DateTime.UtcNow.AddDays(6),
            DurationMinutes = 120,
            DestinationId = 1
        };

        // Act
        var validationResults = ValidateModel(activity);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Name")), Is.True);
    }

    [Test]
    public void Activity_NegativeDuration_FailsValidation()
    {
        // Arrange
        var activity = new Activity
        {
            Name = "Test Activity",
            ScheduledDateTime = DateTime.UtcNow.AddDays(6),
            DurationMinutes = -10,
            DestinationId = 1
        };

        // Act
        var validationResults = ValidateModel(activity);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("DurationMinutes")), Is.True);
    }

    #endregion

    #region Budget Model Tests

    [Test]
    public void Budget_ValidModel_PassesValidation()
    {
        // Arrange
        var budget = new Budget
        {
            Category = "Accommodation",
            PlannedAmount = 1000m,
            ActualAmount = 950m,
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(budget);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void Budget_MissingCategory_FailsValidation()
    {
        // Arrange
        var budget = new Budget
        {
            Category = "",
            PlannedAmount = 1000m,
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(budget);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Category")), Is.True);
    }

    [Test]
    public void Budget_NegativePlannedAmount_FailsValidation()
    {
        // Arrange
        var budget = new Budget
        {
            Category = "Accommodation",
            PlannedAmount = -100m,
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(budget);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("PlannedAmount")), Is.True);
    }

    [Test]
    public void Budget_NegativeActualAmount_FailsValidation()
    {
        // Arrange
        var budget = new Budget
        {
            Category = "Accommodation",
            PlannedAmount = 1000m,
            ActualAmount = -50m,
            TripId = 1
        };

        // Act
        var validationResults = ValidateModel(budget);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("ActualAmount")), Is.True);
    }

    #endregion

    #region TripStatus Constants Tests

    [Test]
    public void TripStatus_ConstantsAreCorrect()
    {
        // Assert
        Assert.That(TripStatus.Planning, Is.EqualTo("Planning"));
        Assert.That(TripStatus.Confirmed, Is.EqualTo("Confirmed"));
        Assert.That(TripStatus.InProgress, Is.EqualTo("InProgress"));
        Assert.That(TripStatus.Completed, Is.EqualTo("Completed"));
        Assert.That(TripStatus.Cancelled, Is.EqualTo("Cancelled"));
    }

    #endregion
}
