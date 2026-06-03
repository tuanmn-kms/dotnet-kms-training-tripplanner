using KMSTraining.API.Application.DTOs;

namespace KMSTraining.API.Application.Services;

public interface ITripService
{
    Task<TripDetailDto?> GetTripByIdAsync(int tripId);
    Task<IEnumerable<TripDto>> GetUserTripsAsync(int userId);
    Task<TripDto> CreateTripAsync(int userId, CreateTripDto dto);
    Task<TripDto> UpdateTripAsync(int tripId, UpdateTripDto dto);
    Task<bool> DeleteTripAsync(int tripId);
    Task<bool> ChangeTripStatusAsync(int tripId, string newStatus);
}

public interface IDestinationService
{
    Task<DestinationDetailDto?> GetDestinationByIdAsync(int destinationId);
    Task<IEnumerable<DestinationDto>> GetTripDestinationsAsync(int tripId);
    Task<DestinationDto> CreateDestinationAsync(int tripId, CreateDestinationDto dto);
    Task<DestinationDto> UpdateDestinationAsync(int destinationId, UpdateDestinationDto dto);
    Task<bool> DeleteDestinationAsync(int destinationId);
}

public interface IActivityService
{
    Task<ActivityDto?> GetActivityByIdAsync(int activityId);
    Task<IEnumerable<ActivityDto>> GetDestinationActivitiesAsync(int destinationId);
    Task<ActivityDto> CreateActivityAsync(int destinationId, CreateActivityDto dto);
    Task<ActivityDto> UpdateActivityAsync(int activityId, UpdateActivityDto dto);
    Task<bool> DeleteActivityAsync(int activityId);
}

public interface IBudgetService
{
    Task<BudgetDto?> GetBudgetByIdAsync(int budgetId);
    Task<IEnumerable<BudgetDto>> GetTripBudgetsAsync(int tripId);
    Task<BudgetDto> CreateBudgetAsync(int tripId, CreateBudgetDto dto);
    Task<BudgetDto> UpdateBudgetAsync(int budgetId, UpdateBudgetDto dto);
    Task<bool> DeleteBudgetAsync(int budgetId);
}
