using KMSTraining.API.Domain.Entities;
using KMSTraining.API.Domain.Interfaces;
using KMSTraining.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KMSTraining.API.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly TripPlannerDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(TripPlannerDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return await Task.FromResult(entity);
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        _dbSet.Remove(entity);
        return true;
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await GetByIdAsync(id) != null;
    }
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(TripPlannerDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}

public class TripRepository : Repository<Trip>, ITripRepository
{
    public TripRepository(TripPlannerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Trip>> GetUserTripsAsync(int userId)
    {
        return await _dbSet
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Trip?> GetTripWithDetailsAsync(int tripId)
    {
        return await _dbSet
            .Include(t => t.Destinations)
                .ThenInclude(d => d.Activities)
            .Include(t => t.Budgets)
            .FirstOrDefaultAsync(t => t.Id == tripId);
    }
}

public class DestinationRepository : Repository<Destination>, IDestinationRepository
{
    public DestinationRepository(TripPlannerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Destination>> GetTripDestinationsAsync(int tripId)
    {
        return await _dbSet
            .Where(d => d.TripId == tripId)
            .Include(d => d.Activities)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<Destination?> GetDestinationWithActivitiesAsync(int destinationId)
    {
        return await _dbSet
            .Include(d => d.Activities)
            .FirstOrDefaultAsync(d => d.Id == destinationId);
    }
}

public class ActivityRepository : Repository<Activity>, IActivityRepository
{
    public ActivityRepository(TripPlannerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Activity>> GetDestinationActivitiesAsync(int destinationId)
    {
        return await _dbSet
            .Where(a => a.DestinationId == destinationId)
            .OrderBy(a => a.ScheduledDateTime)
            .ToListAsync();
    }
}

public class BudgetRepository : Repository<Budget>, IBudgetRepository
{
    public BudgetRepository(TripPlannerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Budget>> GetTripBudgetsAsync(int tripId)
    {
        return await _dbSet
            .Where(b => b.TripId == tripId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }
}
