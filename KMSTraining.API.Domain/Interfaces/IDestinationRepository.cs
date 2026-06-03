using KMSTraining.API.Domain.Entities;

namespace KMSTraining.API.Domain.Interfaces;

/// <summary>
/// Repository interface for Destination entity with specialized queries
/// </summary>
public interface IDestinationRepository : IRepository<Destination>
{
    Task<IEnumerable<Destination>> GetTripDestinationsAsync(int tripId);
    Task<Destination?> GetDestinationWithActivitiesAsync(int destinationId);
}
