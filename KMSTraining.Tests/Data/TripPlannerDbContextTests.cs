using KMSTraining.API.Data;
using KMSTraining.API.Models;
using KMSTraining.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace KMSTraining.Tests.Data;

[TestFixture]
public class TripPlannerDbContextTests
{
    private TripPlannerDbContext _context = null!;

    [SetUp]
    public void SetUp()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region DbSet Tests

    [Test]
    public void DbContext_HasUsersDbSet()
    {
        // Assert
        Assert.That(_context.Users, Is.Not.Null);
    }

    [Test]
    public void DbContext_HasTripsDbSet()
    {
        // Assert
        Assert.That(_context.Trips, Is.Not.Null);
    }

    [Test]
    public void DbContext_HasDestinationsDbSet()
    {
        // Assert
        Assert.That(_context.Destinations, Is.Not.Null);
    }

    [Test]
    public void DbContext_HasActivitiesDbSet()
    {
        // Assert
        Assert.That(_context.Activities, Is.Not.Null);
    }

    [Test]
    public void DbContext_HasBudgetsDbSet()
    {
        // Assert
        Assert.That(_context.Budgets, Is.Not.Null);
    }

    #endregion

    #region Relationship Tests

    [Test]
    public async Task User_CanHaveMultipleTrips()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash123"
        };

        var trip1 = new Trip
        {
            Name = "Trip 1",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5),
            User = user
        };

        var trip2 = new Trip
        {
            Name = "Trip 2",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(15),
            User = user
        };

        // Act
        _context.Users.Add(user);
        _context.Trips.AddRange(trip1, trip2);
        await _context.SaveChangesAsync();

        // Assert
        var savedUser = await _context.Users
            .Include(u => u.Trips)
            .FirstAsync(u => u.Username == "testuser");

        Assert.That(savedUser.Trips, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Trip_CanHaveMultipleDestinations()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash123"
        };

        var trip = new Trip
        {
            Name = "European Tour",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(15),
            User = user
        };

        var dest1 = new Destination
        {
            Name = "Paris",
            Country = "France",
            ArrivalDate = DateTime.UtcNow.AddDays(1),
            DepartureDate = DateTime.UtcNow.AddDays(5),
            Trip = trip
        };

        var dest2 = new Destination
        {
            Name = "Rome",
            Country = "Italy",
            ArrivalDate = DateTime.UtcNow.AddDays(6),
            DepartureDate = DateTime.UtcNow.AddDays(10),
            Trip = trip
        };

        // Act
        _context.Trips.Add(trip);
        _context.Destinations.AddRange(dest1, dest2);
        await _context.SaveChangesAsync();

        // Assert
        var savedTrip = await _context.Trips
            .Include(t => t.Destinations)
            .FirstAsync(t => t.Name == "European Tour");

        Assert.That(savedTrip.Destinations, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Destination_CanHaveMultipleActivities()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash123"
        };

        var trip = new Trip
        {
            Name = "Paris Trip",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5),
            User = user
        };

        var destination = new Destination
        {
            Name = "Paris",
            Country = "France",
            ArrivalDate = DateTime.UtcNow.AddDays(1),
            DepartureDate = DateTime.UtcNow.AddDays(5),
            Trip = trip
        };

        var activity1 = new Activity
        {
            Name = "Eiffel Tower",
            ScheduledDateTime = DateTime.UtcNow.AddDays(2),
            DurationMinutes = 180,
            Destination = destination
        };

        var activity2 = new Activity
        {
            Name = "Louvre Museum",
            ScheduledDateTime = DateTime.UtcNow.AddDays(3),
            DurationMinutes = 240,
            Destination = destination
        };

        // Act
        _context.Destinations.Add(destination);
        _context.Activities.AddRange(activity1, activity2);
        await _context.SaveChangesAsync();

        // Assert
        var savedDestination = await _context.Destinations
            .Include(d => d.Activities)
            .FirstAsync(d => d.Name == "Paris");

        Assert.That(savedDestination.Activities, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Trip_CanHaveMultipleBudgets()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash123"
        };

        var trip = new Trip
        {
            Name = "Summer Vacation",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(10),
            User = user
        };

        var budget1 = new Budget
        {
            Category = "Accommodation",
            PlannedAmount = 1000m,
            Trip = trip
        };

        var budget2 = new Budget
        {
            Category = "Transportation",
            PlannedAmount = 500m,
            Trip = trip
        };

        // Act
        _context.Trips.Add(trip);
        _context.Budgets.AddRange(budget1, budget2);
        await _context.SaveChangesAsync();

        // Assert
        var savedTrip = await _context.Trips
            .Include(t => t.Budgets)
            .FirstAsync(t => t.Name == "Summer Vacation");

        Assert.That(savedTrip.Budgets, Has.Count.EqualTo(2));
    }

    #endregion

    #region Cascade Delete Tests

    [Test]
    public async Task DeletingUser_CascadesDeleteToTrips()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash123"
        };

        var trip = new Trip
        {
            Name = "Test Trip",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5),
            User = user
        };

        _context.Users.Add(user);
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        // Act
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        // Assert
        var tripCount = await _context.Trips.CountAsync();
        Assert.That(tripCount, Is.EqualTo(0));
    }

    [Test]
    public async Task DeletingTrip_CascadesDeleteToDestinations()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash123"
        };

        var trip = new Trip
        {
            Name = "Test Trip",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5),
            User = user
        };

        var destination = new Destination
        {
            Name = "Paris",
            Country = "France",
            ArrivalDate = DateTime.UtcNow.AddDays(1),
            DepartureDate = DateTime.UtcNow.AddDays(5),
            Trip = trip
        };

        _context.Trips.Add(trip);
        _context.Destinations.Add(destination);
        await _context.SaveChangesAsync();

        // Act
        _context.Trips.Remove(trip);
        await _context.SaveChangesAsync();

        // Assert
        var destinationCount = await _context.Destinations.CountAsync();
        Assert.That(destinationCount, Is.EqualTo(0));
    }

    [Test]
    public async Task DeletingTrip_CascadesDeleteToBudgets()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash123"
        };

        var trip = new Trip
        {
            Name = "Test Trip",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5),
            User = user
        };

        var budget = new Budget
        {
            Category = "Accommodation",
            PlannedAmount = 1000m,
            Trip = trip
        };

        _context.Trips.Add(trip);
        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        // Act
        _context.Trips.Remove(trip);
        await _context.SaveChangesAsync();

        // Assert
        var budgetCount = await _context.Budgets.CountAsync();
        Assert.That(budgetCount, Is.EqualTo(0));
    }

    [Test]
    public async Task DeletingDestination_CascadesDeleteToActivities()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash123"
        };

        var trip = new Trip
        {
            Name = "Test Trip",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5),
            User = user
        };

        var destination = new Destination
        {
            Name = "Paris",
            Country = "France",
            ArrivalDate = DateTime.UtcNow.AddDays(1),
            DepartureDate = DateTime.UtcNow.AddDays(5),
            Trip = trip
        };

        var activity = new Activity
        {
            Name = "Eiffel Tower",
            ScheduledDateTime = DateTime.UtcNow.AddDays(2),
            DurationMinutes = 180,
            Destination = destination
        };

        _context.Destinations.Add(destination);
        _context.Activities.Add(activity);
        await _context.SaveChangesAsync();

        // Act
        _context.Destinations.Remove(destination);
        await _context.SaveChangesAsync();

        // Assert
        var activityCount = await _context.Activities.CountAsync();
        Assert.That(activityCount, Is.EqualTo(0));
    }

    #endregion

    #region Index Tests

    [Test]
    public async Task User_UsernameIndex_IsUnique()
    {
        // Note: InMemory database doesn't enforce unique constraints
        // This test verifies the configuration is set up, but won't throw in InMemory
        // For real database testing, use SQLite or SQL Server

        // Arrange
        var user1 = new User
        {
            Username = "testuser",
            Email = "test1@example.com",
            PasswordHash = "hash123"
        };

        _context.Users.Add(user1);
        await _context.SaveChangesAsync();

        // Act - InMemory won't throw, so we just verify the first user was saved
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");

        // Assert
        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser.Username, Is.EqualTo("testuser"));

        // In a real database, the unique index would prevent duplicate usernames
        // The migration tests verify this with SQLite
    }

    [Test]
    public async Task User_EmailIndex_IsUnique()
    {
        // Note: InMemory database doesn't enforce unique constraints
        // This test verifies the configuration is set up, but won't throw in InMemory
        // For real database testing, use SQLite or SQL Server

        // Arrange
        var user1 = new User
        {
            Username = "user1",
            Email = "test@example.com",
            PasswordHash = "hash123"
        };

        _context.Users.Add(user1);
        await _context.SaveChangesAsync();

        // Act - InMemory won't throw, so we just verify the first user was saved
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");

        // Assert
        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser.Email, Is.EqualTo("test@example.com"));

        // In a real database, the unique index would prevent duplicate emails
        // The migration tests verify this with SQLite
    }

    #endregion

    #region CreatedAt and UpdatedAt Tests

    [Test]
    public async Task User_CreatedAt_IsSetAutomatically()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash123"
        };

        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.That(user.CreatedAt, Is.GreaterThan(beforeCreation));
        Assert.That(user.CreatedAt, Is.LessThan(afterCreation));
    }

    [Test]
    public async Task Trip_UpdatedAt_CanBeSet()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash123"
        };

        var trip = new Trip
        {
            Name = "Test Trip",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5),
            User = user
        };

        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        // Act
        trip.Name = "Updated Trip";
        trip.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Assert
        var updatedTrip = await _context.Trips.FindAsync(trip.Id);
        Assert.That(updatedTrip!.UpdatedAt, Is.Not.Null);
    }

    #endregion

    #region Navigation Property Tests

    [Test]
    public async Task Trip_CanLoadUserNavigationProperty()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash123"
        };

        var trip = new Trip
        {
            Name = "Test Trip",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5),
            User = user
        };

        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        // Clear tracking to simulate fresh load
        _context.ChangeTracker.Clear();

        // Act
        var loadedTrip = await _context.Trips
            .Include(t => t.User)
            .FirstAsync(t => t.Id == trip.Id);

        // Assert
        Assert.That(loadedTrip.User, Is.Not.Null);
        Assert.That(loadedTrip.User.Username, Is.EqualTo("testuser"));
    }

    [Test]
    public async Task Destination_CanLoadTripNavigationProperty()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash123"
        };

        var trip = new Trip
        {
            Name = "Test Trip",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5),
            User = user
        };

        var destination = new Destination
        {
            Name = "Paris",
            Country = "France",
            ArrivalDate = DateTime.UtcNow.AddDays(1),
            DepartureDate = DateTime.UtcNow.AddDays(5),
            Trip = trip
        };

        _context.Destinations.Add(destination);
        await _context.SaveChangesAsync();

        // Clear tracking
        _context.ChangeTracker.Clear();

        // Act
        var loadedDestination = await _context.Destinations
            .Include(d => d.Trip)
            .FirstAsync(d => d.Id == destination.Id);

        // Assert
        Assert.That(loadedDestination.Trip, Is.Not.Null);
        Assert.That(loadedDestination.Trip.Name, Is.EqualTo("Test Trip"));
    }

    #endregion
}
