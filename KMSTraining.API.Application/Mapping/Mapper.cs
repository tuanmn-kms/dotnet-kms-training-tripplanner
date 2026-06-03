using KMSTraining.API.Application.DTOs;
using KMSTraining.API.Domain.Entities;

namespace KMSTraining.API.Application.Mapping;

public interface IMapper
{
    // User mappings
    UserDto MapUserToDto(User user);
    User MapRegisterDtoToUser(string passwordHash, RegisterDto dto);

    // Trip mappings
    TripDto MapTripToDto(Trip trip);
    TripDetailDto MapTripToDetailDto(Trip trip);
    Trip MapCreateTripDtoToTrip(CreateTripDto dto, int userId);

    // Destination mappings
    DestinationDto MapDestinationToDto(Destination destination);
    DestinationDetailDto MapDestinationToDetailDto(Destination destination);
    Destination MapCreateDestinationDtoToDestination(CreateDestinationDto dto, int tripId);

    // Activity mappings
    ActivityDto MapActivityToDto(Activity activity);
    Activity MapCreateActivityDtoToActivity(CreateActivityDto dto, int destinationId);

    // Budget mappings
    BudgetDto MapBudgetToDto(Budget budget);
    Budget MapCreateBudgetDtoToBudget(CreateBudgetDto dto, int tripId);
}

public class Mapper : IMapper
{
    public UserDto MapUserToDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        CreatedAt = user.CreatedAt
    };

    public User MapRegisterDtoToUser(string passwordHash, RegisterDto dto) =>
        User.Create(dto.Username, dto.Email, passwordHash, dto.FirstName, dto.LastName);

    public TripDto MapTripToDto(Trip trip) => new()
    {
        Id = trip.Id,
        Name = trip.Name,
        Description = trip.Description,
        StartDate = trip.StartDate,
        EndDate = trip.EndDate,
        Status = trip.Status,
        UserId = trip.UserId,
        CreatedAt = trip.CreatedAt,
        UpdatedAt = trip.UpdatedAt
    };

    public TripDetailDto MapTripToDetailDto(Trip trip) => new()
    {
        Id = trip.Id,
        Name = trip.Name,
        Description = trip.Description,
        StartDate = trip.StartDate,
        EndDate = trip.EndDate,
        Status = trip.Status,
        UserId = trip.UserId,
        CreatedAt = trip.CreatedAt,
        UpdatedAt = trip.UpdatedAt,
        Destinations = trip.Destinations.Select(MapDestinationToDto).ToList(),
        Budgets = trip.Budgets.Select(MapBudgetToDto).ToList()
    };

    public Trip MapCreateTripDtoToTrip(CreateTripDto dto, int userId) =>
        Trip.Create(dto.Name, userId, dto.StartDate, dto.EndDate, dto.Description);

    public DestinationDto MapDestinationToDto(Destination destination) => new()
    {
        Id = destination.Id,
        Name = destination.Name,
        Country = destination.Country,
        City = destination.City,
        Description = destination.Description,
        ArrivalDate = destination.ArrivalDate,
        DepartureDate = destination.DepartureDate,
        TripId = destination.TripId,
        CreatedAt = destination.CreatedAt,
        UpdatedAt = destination.UpdatedAt
    };

    public DestinationDetailDto MapDestinationToDetailDto(Destination destination) => new()
    {
        Id = destination.Id,
        Name = destination.Name,
        Country = destination.Country,
        City = destination.City,
        Description = destination.Description,
        ArrivalDate = destination.ArrivalDate,
        DepartureDate = destination.DepartureDate,
        TripId = destination.TripId,
        CreatedAt = destination.CreatedAt,
        UpdatedAt = destination.UpdatedAt,
        Activities = destination.Activities.Select(MapActivityToDto).ToList()
    };

    public Destination MapCreateDestinationDtoToDestination(CreateDestinationDto dto, int tripId) =>
        Destination.Create(dto.Name, dto.Country, dto.ArrivalDate, dto.DepartureDate, tripId, dto.City, dto.Description);

    public ActivityDto MapActivityToDto(Activity activity) => new()
    {
        Id = activity.Id,
        Name = activity.Name,
        Description = activity.Description,
        ScheduledDateTime = activity.ScheduledDateTime,
        DurationMinutes = activity.DurationMinutes,
        Location = activity.Location,
        EstimatedCost = activity.EstimatedCost,
        DestinationId = activity.DestinationId,
        CreatedAt = activity.CreatedAt,
        UpdatedAt = activity.UpdatedAt
    };

    public Activity MapCreateActivityDtoToActivity(CreateActivityDto dto, int destinationId) =>
        Activity.Create(
            dto.Name,
            dto.ScheduledDateTime,
            destinationId,
            dto.Description,
            dto.DurationMinutes,
            dto.Location,
            dto.EstimatedCost);

    public BudgetDto MapBudgetToDto(Budget budget) => new()
    {
        Id = budget.Id,
        Category = budget.Category,
        PlannedAmount = budget.PlannedAmount,
        ActualAmount = budget.ActualAmount,
        Notes = budget.Notes,
        TripId = budget.TripId,
        CreatedAt = budget.CreatedAt,
        UpdatedAt = budget.UpdatedAt
    };

    public Budget MapCreateBudgetDtoToBudget(CreateBudgetDto dto, int tripId) =>
        Budget.Create(dto.Category, dto.PlannedAmount, tripId, dto.ActualAmount, dto.Notes);
}
