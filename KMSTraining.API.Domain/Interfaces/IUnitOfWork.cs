namespace KMSTraining.API.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern interface to manage multiple repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ITripRepository Trips { get; }
    IDestinationRepository Destinations { get; }
    IActivityRepository Activities { get; }
    IBudgetRepository Budgets { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
