using KMSTraining.API.Domain.Entities;

namespace KMSTraining.API.Domain.Interfaces;

/// <summary>
/// Repository interface for Budget entity with specialized queries
/// </summary>
public interface IBudgetRepository : IRepository<Budget>
{
    Task<IEnumerable<Budget>> GetTripBudgetsAsync(int tripId);
}
