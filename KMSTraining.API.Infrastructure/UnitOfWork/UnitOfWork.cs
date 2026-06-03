using KMSTraining.API.Domain.Interfaces;
using KMSTraining.API.Infrastructure.Data;
using KMSTraining.API.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace KMSTraining.API.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly TripPlannerDbContext _context;
    private IDbContextTransaction? _transaction;

    private IUserRepository? _userRepository;
    private ITripRepository? _tripRepository;
    private IDestinationRepository? _destinationRepository;
    private IActivityRepository? _activityRepository;
    private IBudgetRepository? _budgetRepository;

    public UnitOfWork(TripPlannerDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _userRepository ??= new UserRepository(_context);
    public ITripRepository Trips => _tripRepository ??= new TripRepository(_context);
    public IDestinationRepository Destinations => _destinationRepository ??= new DestinationRepository(_context);
    public IActivityRepository Activities => _activityRepository ??= new ActivityRepository(_context);
    public IBudgetRepository Budgets => _budgetRepository ??= new BudgetRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackAsync()
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }
}
