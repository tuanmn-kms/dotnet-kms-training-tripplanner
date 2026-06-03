using KMSTraining.API.Domain.Entities;

namespace KMSTraining.API.Domain.Interfaces;

/// <summary>
/// Repository interface for Trip entity with specialized queries
/// </summary>
public interface ITripRepository : IRepository<Trip>
{
    Task<IEnumerable<Trip>> GetUserTripsAsync(int userId);
    Task<Trip?> GetTripWithDetailsAsync(int tripId);
}
