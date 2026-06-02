using KMSTraining.API.Controllers;
using KMSTraining.API.Data;
using KMSTraining.API.DTOs;
using KMSTraining.API.Models;
using KMSTraining.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMSTraining.Tests.Controllers;

[TestFixture]
public class BudgetsControllerTests
{
    private TripPlannerDbContext _context = null!;
    private BudgetsController _controller = null!;
    private User _testUser = null!;
    private Trip _testTrip = null!;

    [SetUp]
    public void SetUp()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _controller = new BudgetsController(_context);

        // Create test data
        _testUser = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash"
        };
        _context.Users.Add(_testUser);

        _testTrip = new Trip
        {
            Id = 1,
            Name = "Test Trip",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            UserId = _testUser.Id
        };
        _context.Trips.Add(_testTrip);
        _context.SaveChanges();

        // Setup claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _testUser.Id.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetBudgets_ReturnsAllBudgets()
    {
        // Arrange
        _context.Budgets.AddRange(
            new Budget
            {
                Category = "Accommodation",
                PlannedAmount = 1000.00m,
                ActualAmount = 950.00m,
                TripId = _testTrip.Id
            },
            new Budget
            {
                Category = "Food",
                PlannedAmount = 500.00m,
                ActualAmount = 450.00m,
                TripId = _testTrip.Id
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetBudgets();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var budgets = okResult!.Value as IEnumerable<BudgetDto>;
        Assert.That(budgets!.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetBudgets_WithTripId_ReturnsFilteredBudgets()
    {
        // Arrange
        var anotherTrip = new Trip
        {
            Name = "Another Trip",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            UserId = _testUser.Id
        };
        _context.Trips.Add(anotherTrip);
        await _context.SaveChangesAsync();

        _context.Budgets.AddRange(
            new Budget
            {
                Category = "Accommodation",
                PlannedAmount = 1000.00m,
                TripId = _testTrip.Id
            },
            new Budget
            {
                Category = "Transport",
                PlannedAmount = 800.00m,
                TripId = anotherTrip.Id
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetBudgets(_testTrip.Id);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var budgets = okResult!.Value as IEnumerable<BudgetDto>;
        Assert.That(budgets!.Count(), Is.EqualTo(1));
        Assert.That(budgets?.First().Category, Is.EqualTo("Accommodation"));
    }

    [Test]
    public async Task GetBudget_ExistingBudget_ReturnsBudget()
    {
        // Arrange
        var budget = new Budget
        {
            Category = "Accommodation",
            PlannedAmount = 1500.00m,
            ActualAmount = 1400.00m,
            Notes = "Hotels and Airbnb",
            TripId = _testTrip.Id
        };
        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetBudget(budget.Id);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var budgetDto = okResult!.Value as BudgetDto;
        Assert.That(budgetDto!.Category, Is.EqualTo("Accommodation"));
        Assert.That(budgetDto.PlannedAmount, Is.EqualTo(1500.00m));
    }

    [Test]
    public async Task CreateBudget_ValidBudget_ReturnsCreated()
    {
        // Arrange
        var createDto = new CreateBudgetDto
        {
            Category = "Entertainment",
            PlannedAmount = 300.00m,
            ActualAmount = 0.00m,
            Notes = "Shows and attractions",
            TripId = _testTrip.Id
        };

        // Act
        var result = await _controller.CreateBudget(createDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = result.Result as CreatedAtActionResult;
        var budgetDto = createdResult!.Value as BudgetDto;
        Assert.That(budgetDto!.Category, Is.EqualTo("Entertainment"));
        Assert.That(budgetDto.PlannedAmount, Is.EqualTo(300.00m));
    }

    [Test]
    public async Task CreateBudget_NonExistentTrip_ReturnsNotFound()
    {
        // Arrange
        var createDto = new CreateBudgetDto
        {
            Category = "Test",
            PlannedAmount = 100.00m,
            TripId = 999
        };

        // Act
        var result = await _controller.CreateBudget(createDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task UpdateBudget_ValidUpdate_ReturnsUpdated()
    {
        // Arrange
        var budget = new Budget
        {
            Category = "Food",
            PlannedAmount = 500.00m,
            ActualAmount = 0.00m,
            TripId = _testTrip.Id
        };
        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateBudgetDto
        {
            ActualAmount = 475.00m,
            Notes = "Spent less than planned"
        };

        // Act
        var result = await _controller.UpdateBudget(budget.Id, updateDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var budgetDto = okResult!.Value as BudgetDto;
        Assert.That(budgetDto!.ActualAmount, Is.EqualTo(475.00m));
        Assert.That(budgetDto.Notes, Is.EqualTo("Spent less than planned"));
    }

    [Test]
    public async Task UpdateBudget_NonExistentBudget_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateBudgetDto
        {
            ActualAmount = 100.00m
        };

        // Act
        var result = await _controller.UpdateBudget(999, updateDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task DeleteBudget_ExistingBudget_ReturnsNoContent()
    {
        // Arrange
        var budget = new Budget
        {
            Category = "To Delete",
            PlannedAmount = 100.00m,
            TripId = _testTrip.Id
        };
        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteBudget(budget.Id);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());

        var deletedBudget = await _context.Budgets.FindAsync(budget.Id);
        Assert.That(deletedBudget, Is.Null);
    }

    [Test]
    public async Task DeleteBudget_NonExistentBudget_ReturnsNotFound()
    {
        // Act
        var result = await _controller.DeleteBudget(999);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task CreateBudget_WithZeroPlannedAmount_ReturnsCreated()
    {
        // Arrange
        var createDto = new CreateBudgetDto
        {
            Category = "Miscellaneous",
            PlannedAmount = 0.00m,
            ActualAmount = 0.00m,
            TripId = _testTrip.Id
        };

        // Act
        var result = await _controller.CreateBudget(createDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
    }
}
