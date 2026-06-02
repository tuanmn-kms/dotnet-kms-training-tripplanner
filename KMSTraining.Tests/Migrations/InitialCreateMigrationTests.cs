using KMSTraining.API.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace KMSTraining.Tests.Migrations;

[TestFixture]
public class InitialCreateMigrationTests
{
    private SqliteConnection _connection = null!;
    private DbContextOptions<TripPlannerDbContext> _contextOptions = null!;

    [SetUp]
    public void Setup()
    {
        // Create and open a connection. This creates the SQLite in-memory database
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // These options will be used by the context instances in this test suite
        _contextOptions = new DbContextOptionsBuilder<TripPlannerDbContext>()
            .UseSqlite(_connection)
            .Options;
    }

    [TearDown]
    public void TearDown()
    {
        _connection?.Dispose();
    }

    [Test]
    public void Migration_ShouldCreateUsersTable()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Verify Users table exists
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Users'";
        var result = command.ExecuteScalar();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo("Users"));
    }

    [Test]
    public async Task Migration_ShouldCreateUsersTableWithCorrectColumns()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Insert a test user to verify columns
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, CreatedAt, UpdatedAt)
            VALUES ('testuser', 'test@example.com', 'hashedpassword', 'John', 'Doe', '2026-01-01', '2026-01-02')
        ");

        var command = _connection.CreateCommand();
        command.CommandText = "SELECT Username, Email, PasswordHash, FirstName, LastName FROM Users WHERE Username = 'testuser'";
        using var reader = await command.ExecuteReaderAsync();
        var hasData = await reader.ReadAsync();

        Assert.That(hasData, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(reader.GetString(0), Is.EqualTo("testuser"));
            Assert.That(reader.GetString(1), Is.EqualTo("test@example.com"));
            Assert.That(reader.GetString(2), Is.EqualTo("hashedpassword"));
            Assert.That(reader.GetString(3), Is.EqualTo("John"));
            Assert.That(reader.GetString(4), Is.EqualTo("Doe"));
        });
    }

    [Test]
    public void Migration_ShouldCreateTripsTable()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Verify Trips table exists
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Trips'";
        var result = command.ExecuteScalar();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo("Trips"));
    }

    [Test]
    public async Task Migration_ShouldCreateTripsTableWithCorrectColumns()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Insert test data
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
            VALUES ('testuser', 'test@example.com', 'hashedpassword', '2026-01-01')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Trips (Name, Description, StartDate, EndDate, Status, UserId, CreatedAt, UpdatedAt)
            VALUES ('Test Trip', 'A test trip', '2026-06-01', '2026-06-10', 'Planned', 1, '2026-01-01', '2026-01-02')
        ");

        var command = _connection.CreateCommand();
        command.CommandText = "SELECT Name, Description, Status FROM Trips WHERE Name = 'Test Trip'";
        using var reader = await command.ExecuteReaderAsync();
        var hasData = await reader.ReadAsync();

        Assert.That(hasData, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(reader.GetString(0), Is.EqualTo("Test Trip"));
            Assert.That(reader.GetString(1), Is.EqualTo("A test trip"));
            Assert.That(reader.GetString(2), Is.EqualTo("Planned"));
        });
    }

    [Test]
    public void Migration_ShouldCreateDestinationsTable()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Verify Destinations table exists
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Destinations'";
        var result = command.ExecuteScalar();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo("Destinations"));
    }

    [Test]
    public async Task Migration_ShouldCreateDestinationsTableWithCorrectColumns()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Insert test data
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
            VALUES ('testuser', 'test@example.com', 'hashedpassword', '2026-01-01')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Trips (Name, StartDate, EndDate, Status, UserId, CreatedAt)
            VALUES ('Test Trip', '2026-06-01', '2026-06-10', 'Planned', 1, '2026-01-01')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Destinations (Name, Country, City, Description, ArrivalDate, DepartureDate, TripId, CreatedAt, UpdatedAt)
            VALUES ('Paris', 'France', 'Paris', 'City of lights', '2026-06-01', '2026-06-05', 1, '2026-01-01', '2026-01-02')
        ");

        var command = _connection.CreateCommand();
        command.CommandText = "SELECT Name, Country, City, Description FROM Destinations WHERE Name = 'Paris'";
        using var reader = await command.ExecuteReaderAsync();
        var hasData = await reader.ReadAsync();

        Assert.That(hasData, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(reader.GetString(0), Is.EqualTo("Paris"));
            Assert.That(reader.GetString(1), Is.EqualTo("France"));
            Assert.That(reader.GetString(2), Is.EqualTo("Paris"));
            Assert.That(reader.GetString(3), Is.EqualTo("City of lights"));
        });
    }

    [Test]
    public void Migration_ShouldCreateActivitiesTable()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Verify Activities table exists
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Activities'";
        var result = command.ExecuteScalar();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo("Activities"));
    }

    [Test]
    public async Task Migration_ShouldCreateActivitiesTableWithCorrectColumns()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Insert test data
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
            VALUES ('testuser', 'test@example.com', 'hashedpassword', '2026-01-01')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Trips (Name, StartDate, EndDate, Status, UserId, CreatedAt)
            VALUES ('Test Trip', '2026-06-01', '2026-06-10', 'Planned', 1, '2026-01-01')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Destinations (Name, Country, ArrivalDate, DepartureDate, TripId, CreatedAt)
            VALUES ('Paris', 'France', '2026-06-01', '2026-06-05', 1, '2026-01-01')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Activities (Name, Description, ScheduledDateTime, DurationMinutes, Location, EstimatedCost, DestinationId, CreatedAt, UpdatedAt)
            VALUES ('Eiffel Tower Visit', 'Visit the Eiffel Tower', '2026-06-02 10:00:00', 120, 'Eiffel Tower', 25.50, 1, '2026-01-01', '2026-01-02')
        ");

        var command = _connection.CreateCommand();
        command.CommandText = "SELECT Name, Description, DurationMinutes, Location FROM Activities WHERE Name = 'Eiffel Tower Visit'";
        using var reader = await command.ExecuteReaderAsync();
        var hasData = await reader.ReadAsync();

        Assert.That(hasData, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(reader.GetString(0), Is.EqualTo("Eiffel Tower Visit"));
            Assert.That(reader.GetString(1), Is.EqualTo("Visit the Eiffel Tower"));
            Assert.That(reader.GetInt32(2), Is.EqualTo(120));
            Assert.That(reader.GetString(3), Is.EqualTo("Eiffel Tower"));
        });
    }

    [Test]
    public void Migration_ShouldCreateBudgetsTable()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Verify Budgets table exists
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Budgets'";
        var result = command.ExecuteScalar();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo("Budgets"));
    }

    [Test]
    public async Task Migration_ShouldCreateBudgetsTableWithCorrectColumns()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Insert test data
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
            VALUES ('testuser', 'test@example.com', 'hashedpassword', '2026-01-01')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Trips (Name, StartDate, EndDate, Status, UserId, CreatedAt)
            VALUES ('Test Trip', '2026-06-01', '2026-06-10', 'Planned', 1, '2026-01-01')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Budgets (Category, PlannedAmount, ActualAmount, Notes, TripId, CreatedAt, UpdatedAt)
            VALUES ('Accommodation', 500.00, 450.00, 'Hotel expenses', 1, '2026-01-01', '2026-01-02')
        ");

        var command = _connection.CreateCommand();
        command.CommandText = "SELECT Category, PlannedAmount, ActualAmount, Notes FROM Budgets WHERE Category = 'Accommodation'";
        using var reader = await command.ExecuteReaderAsync();
        var hasData = await reader.ReadAsync();

        Assert.That(hasData, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(reader.GetString(0), Is.EqualTo("Accommodation"));
            Assert.That(reader.GetDecimal(1), Is.EqualTo(500.00m));
            Assert.That(reader.GetDecimal(2), Is.EqualTo(450.00m));
            Assert.That(reader.GetString(3), Is.EqualTo("Hotel expenses"));
        });
    }

    [Test]
    public async Task Migration_ShouldEnforceForeignKeyConstraints()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Insert valid parent record
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
            VALUES ('testuser', 'test@example.com', 'hashedpassword', '2026-01-01')
        ");

        // Try to insert Trip with valid UserId - should succeed
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Trips (Name, StartDate, EndDate, Status, UserId, CreatedAt)
            VALUES ('Test Trip', '2026-06-01', '2026-06-10', 'Planned', 1, '2026-01-01')
        ");

        // Try to insert Trip with invalid UserId - should fail
        var ex = Assert.ThrowsAsync<Microsoft.Data.Sqlite.SqliteException>(async () =>
            await context.Database.ExecuteSqlRawAsync(@"
                INSERT INTO Trips (Name, StartDate, EndDate, Status, UserId, CreatedAt)
                VALUES ('Invalid Trip', '2026-06-01', '2026-06-10', 'Planned', 999, '2026-01-01')
            ")
        );

        Assert.That(ex?.Message, Does.Contain("FOREIGN KEY constraint failed").IgnoreCase);
    }

    [Test]
    public async Task Migration_ShouldEnforceUniqueConstraintOnUsername()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Insert first user
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
            VALUES ('testuser', 'test1@example.com', 'hashedpassword', '2026-01-01')
        ");

        // Try to insert duplicate username - should fail
        var ex = Assert.ThrowsAsync<Microsoft.Data.Sqlite.SqliteException>(async () =>
            await context.Database.ExecuteSqlRawAsync(@"
                INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
                VALUES ('testuser', 'test2@example.com', 'hashedpassword', '2026-01-01')
            ")
        );

        Assert.That(ex?.Message, Does.Contain("UNIQUE constraint failed").IgnoreCase);
    }

    [Test]
    public async Task Migration_ShouldEnforceUniqueConstraintOnEmail()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Insert first user
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
            VALUES ('testuser1', 'test@example.com', 'hashedpassword', '2026-01-01')
        ");

        // Try to insert duplicate email - should fail
        var ex = Assert.ThrowsAsync<Microsoft.Data.Sqlite.SqliteException>(async () =>
            await context.Database.ExecuteSqlRawAsync(@"
                INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
                VALUES ('testuser2', 'test@example.com', 'hashedpassword', '2026-01-01')
            ")
        );

        Assert.That(ex?.Message, Does.Contain("UNIQUE constraint failed").IgnoreCase);
    }

    [Test]
    public async Task Migration_ShouldImplementCascadeDeleteForTrips()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Insert test data
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
            VALUES ('testuser', 'test@example.com', 'hashedpassword', '2026-01-01')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Trips (Name, StartDate, EndDate, Status, UserId, CreatedAt)
            VALUES ('Test Trip', '2026-06-01', '2026-06-10', 'Planned', 1, '2026-01-01')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Destinations (Name, Country, ArrivalDate, DepartureDate, TripId, CreatedAt)
            VALUES ('Paris', 'France', '2026-06-01', '2026-06-05', 1, '2026-01-01')
        ");

        // Delete the trip
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Trips WHERE Id = 1");

        // Verify destination was also deleted (cascade)
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Destinations WHERE TripId = 1";
        var count = (long)(command.ExecuteScalar() ?? 0L);

        Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public async Task Migration_ShouldImplementCascadeDeleteForDestinations()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Insert test data
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
            VALUES ('testuser', 'test@example.com', 'hashedpassword', '2026-01-01')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Trips (Name, StartDate, EndDate, Status, UserId, CreatedAt)
            VALUES ('Test Trip', '2026-06-01', '2026-06-10', 'Planned', 1, '2026-01-01')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Destinations (Name, Country, ArrivalDate, DepartureDate, TripId, CreatedAt)
            VALUES ('Paris', 'France', '2026-06-01', '2026-06-05', 1, '2026-01-01')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Activities (Name, ScheduledDateTime, DurationMinutes, DestinationId, CreatedAt)
            VALUES ('Louvre Museum', '2026-06-02 14:00:00', 180, 1, '2026-01-02')
        ");

        // Delete the destination
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Destinations WHERE Id = 1");

        // Verify activity was also deleted (cascade)
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Activities WHERE DestinationId = 1";
        var count = (long)(command.ExecuteScalar() ?? 0L);

        Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public async Task Migration_ShouldSupportCompleteWorkflow()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        // Insert a complete workflow: User -> Trip -> Destination -> Activity + Budget
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, CreatedAt)
            VALUES ('traveler', 'traveler@example.com', 'hash123', 'Jane', 'Doe', '2026-01-01')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Trips (Name, Description, StartDate, EndDate, Status, UserId, CreatedAt)
            VALUES ('European Adventure', 'Summer trip to Europe', '2026-06-01', '2026-06-15', 'Planned', 1, '2026-01-02')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Destinations (Name, Country, City, ArrivalDate, DepartureDate, TripId, CreatedAt)
            VALUES ('Paris', 'France', 'Paris', '2026-06-01', '2026-06-05', 1, '2026-01-02')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Activities (Name, ScheduledDateTime, DurationMinutes, DestinationId, CreatedAt)
            VALUES ('Louvre Museum', '2026-06-02 14:00:00', 180, 1, '2026-01-02')
        ");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Budgets (Category, PlannedAmount, ActualAmount, TripId, CreatedAt)
            VALUES ('Transportation', 800.00, 0.00, 1, '2026-01-02')
        ");

        // Verify all records exist
        var command = _connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                (SELECT COUNT(*) FROM Users) as UserCount,
                (SELECT COUNT(*) FROM Trips) as TripCount,
                (SELECT COUNT(*) FROM Destinations) as DestinationCount,
                (SELECT COUNT(*) FROM Activities) as ActivityCount,
                (SELECT COUNT(*) FROM Budgets) as BudgetCount
        ";

        using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.Multiple(() =>
        {
            Assert.That(reader.GetInt64(0), Is.EqualTo(1), "User count should be 1");
            Assert.That(reader.GetInt64(1), Is.EqualTo(1), "Trip count should be 1");
            Assert.That(reader.GetInt64(2), Is.EqualTo(1), "Destination count should be 1");
            Assert.That(reader.GetInt64(3), Is.EqualTo(1), "Activity count should be 1");
            Assert.That(reader.GetInt64(4), Is.EqualTo(1), "Budget count should be 1");
        });
    }

    [Test]
    public void Migration_ShouldCreateAllRequiredIndexes()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        var command = _connection.CreateCommand();
        command.CommandText = @"
            SELECT name FROM sqlite_master 
            WHERE type='index' 
            AND name IN (
                'IX_Users_Username',
                'IX_Users_Email',
                'IX_Trips_UserId',
                'IX_Destinations_TripId',
                'IX_Activities_DestinationId',
                'IX_Budgets_TripId'
            )
            ORDER BY name
        ";

        var indexes = new List<string>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            indexes.Add(reader.GetString(0));
        }

        Assert.Multiple(() =>
        {
            Assert.That(indexes, Does.Contain("IX_Activities_DestinationId"));
            Assert.That(indexes, Does.Contain("IX_Budgets_TripId"));
            Assert.That(indexes, Does.Contain("IX_Destinations_TripId"));
            Assert.That(indexes, Does.Contain("IX_Trips_UserId"));
            Assert.That(indexes, Does.Contain("IX_Users_Email"));
            Assert.That(indexes, Does.Contain("IX_Users_Username"));
        });
    }

    [Test]
    public void Migration_ShouldCreateAllRequiredTables()
    {
        using var context = new TripPlannerDbContext(_contextOptions);
        context.Database.EnsureCreated();

        var command = _connection.CreateCommand();
        command.CommandText = @"
            SELECT name FROM sqlite_master 
            WHERE type='table' 
            AND name IN ('Users', 'Trips', 'Destinations', 'Activities', 'Budgets')
            ORDER BY name
        ";

        var tables = new List<string>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            tables.Add(reader.GetString(0));
        }

        Assert.Multiple(() =>
        {
            Assert.That(tables, Has.Count.EqualTo(5));
            Assert.That(tables, Does.Contain("Activities"));
            Assert.That(tables, Does.Contain("Budgets"));
            Assert.That(tables, Does.Contain("Destinations"));
            Assert.That(tables, Does.Contain("Trips"));
            Assert.That(tables, Does.Contain("Users"));
        });
    }
}

