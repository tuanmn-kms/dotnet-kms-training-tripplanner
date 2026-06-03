using KMSTraining.API.Application.DTOs;
using KMSTraining.API.Application.Services;
using KMSTraining.API.Controllers;
using KMSTraining.API.Domain.Entities;
using KMSTraining.API.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace KMSTraining.Tests.Controllers;

[TestFixture]
public class BudgetsControllerTests
{
    private Mock<IBudgetService> _budgetService = null!;
    private BudgetsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _budgetService = new Mock<IBudgetService>();
        _controller = new BudgetsController(_budgetService.Object, Mock.Of<ILogger<BudgetsController>>());
    }

    [Test]
    public async Task GetBudgets_WithTripId_ReturnsBudgets()
    {
        var budgets = new[]
        {
            new BudgetDto { Id = 1, Category = "Food", PlannedAmount = 100, TripId = 3 }
        };

        _budgetService.Setup(s => s.GetTripBudgetsAsync(3)).ReturnsAsync(budgets);

        var result = await _controller.GetBudgets(3);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        _budgetService.Verify(s => s.GetTripBudgetsAsync(3), Times.Once);
    }

    [Test]
    public async Task GetBudget_WhenMissing_ReturnsNotFound()
    {
        _budgetService.Setup(s => s.GetBudgetByIdAsync(404)).ReturnsAsync((BudgetDto?)null);

        var result = await _controller.GetBudget(404);

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task CreateBudget_UsesDtoTripIdAndReturnsCreated()
    {
        var request = new CreateBudgetDto
        {
            Category = "Lodging",
            PlannedAmount = 500,
            TripId = 6
        };

        _budgetService.Setup(s => s.CreateBudgetAsync(6, request))
            .ReturnsAsync(new BudgetDto { Id = 30, Category = "Lodging", PlannedAmount = 500, TripId = 6 });

        var result = await _controller.CreateBudget(request);

        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        _budgetService.Verify(s => s.CreateBudgetAsync(6, request), Times.Once);
    }

    [Test]
    public async Task UpdateBudget_WhenMissing_ReturnsNotFound()
    {
        _budgetService.Setup(s => s.UpdateBudgetAsync(404, It.IsAny<UpdateBudgetDto>()))
            .ThrowsAsync(new EntityNotFoundException(nameof(Budget), 404));

        var result = await _controller.UpdateBudget(404, new UpdateBudgetDto { ActualAmount = 20 });

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task DeleteBudget_WhenDeleted_ReturnsNoContent()
    {
        _budgetService.Setup(s => s.DeleteBudgetAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteBudget(1);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}
