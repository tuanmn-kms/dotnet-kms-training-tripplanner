using KMSTraining.API.Domain.Entities;

namespace KMSTraining.API.Domain.Interfaces;

/// <summary>
/// Repository interface for Activity entity with specialized queries
/// </summary>
public interface IActivityRepository : IRepository<Activity>
{
    Task<IEnumerable<Activity>> GetDestinationActivitiesAsync(int destinationId);
}
