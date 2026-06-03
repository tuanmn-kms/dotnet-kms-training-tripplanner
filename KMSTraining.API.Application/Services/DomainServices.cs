using KMSTraining.API.Application.DTOs;
using KMSTraining.API.Application.Mapping;
using KMSTraining.API.Domain.Exceptions;
using KMSTraining.API.Domain.Interfaces;

namespace KMSTraining.API.Application.Services;

public class TripService : ITripService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TripService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TripDetailDto?> GetTripByIdAsync(int tripId)
    {
        var trip = await _unitOfWork.Trips.GetTripWithDetailsAsync(tripId);
        return trip != null ? _mapper.MapTripToDetailDto(trip) : null;
    }

    public async Task<IEnumerable<TripDto>> GetUserTripsAsync(int userId)
    {
        var trips = await _unitOfWork.Trips.GetUserTripsAsync(userId);
        return trips.Select(_mapper.MapTripToDto);
    }

    public async Task<TripDto> CreateTripAsync(int userId, CreateTripDto dto)
    {
        var trip = _mapper.MapCreateTripDtoToTrip(dto, userId);
        var createdTrip = await _unitOfWork.Trips.AddAsync(trip);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.MapTripToDto(createdTrip);
    }

    public async Task<TripDto> UpdateTripAsync(int tripId, UpdateTripDto dto)
    {
        var trip = await _unitOfWork.Trips.GetByIdAsync(tripId)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Trip), tripId);

        trip.Update(
            dto.Name ?? trip.Name,
            dto.StartDate ?? trip.StartDate,
            dto.EndDate ?? trip.EndDate,
            dto.Description ?? trip.Description);

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            trip.ChangeStatus(dto.Status);
        }

        var updatedTrip = await _unitOfWork.Trips.UpdateAsync(trip);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.MapTripToDto(updatedTrip);
    }

    public async Task<bool> DeleteTripAsync(int tripId)
    {
        var result = await _unitOfWork.Trips.DeleteAsync(tripId);
        if (result)
        {
            await _unitOfWork.SaveChangesAsync();
        }
        return result;
    }

    public async Task<bool> ChangeTripStatusAsync(int tripId, string newStatus)
    {
        var trip = await _unitOfWork.Trips.GetByIdAsync(tripId)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Trip), tripId);

        trip.ChangeStatus(newStatus);
        await _unitOfWork.Trips.UpdateAsync(trip);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}

public class DestinationService : IDestinationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DestinationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DestinationDetailDto?> GetDestinationByIdAsync(int destinationId)
    {
        var destination = await _unitOfWork.Destinations.GetDestinationWithActivitiesAsync(destinationId);
        return destination != null ? _mapper.MapDestinationToDetailDto(destination) : null;
    }

    public async Task<IEnumerable<DestinationDto>> GetTripDestinationsAsync(int tripId)
    {
        var destinations = tripId > 0
            ? await _unitOfWork.Destinations.GetTripDestinationsAsync(tripId)
            : await _unitOfWork.Destinations.GetAllAsync();

        return destinations.Select(_mapper.MapDestinationToDto);
    }

    public async Task<DestinationDto> CreateDestinationAsync(int tripId, CreateDestinationDto dto)
    {
        var destination = _mapper.MapCreateDestinationDtoToDestination(dto, tripId);
        var createdDestination = await _unitOfWork.Destinations.AddAsync(destination);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.MapDestinationToDto(createdDestination);
    }

    public async Task<DestinationDto> UpdateDestinationAsync(int destinationId, UpdateDestinationDto dto)
    {
        var destination = await _unitOfWork.Destinations.GetByIdAsync(destinationId)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Destination), destinationId);

        destination.Update(
            dto.Name,
            dto.Country,
            dto.ArrivalDate,
            dto.DepartureDate,
            dto.City,
            dto.Description);

        var updatedDestination = await _unitOfWork.Destinations.UpdateAsync(destination);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.MapDestinationToDto(updatedDestination);
    }

    public async Task<bool> DeleteDestinationAsync(int destinationId)
    {
        var result = await _unitOfWork.Destinations.DeleteAsync(destinationId);
        if (result)
        {
            await _unitOfWork.SaveChangesAsync();
        }
        return result;
    }
}

public class ActivityService : IActivityService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ActivityService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ActivityDto?> GetActivityByIdAsync(int activityId)
    {
        var activity = await _unitOfWork.Activities.GetByIdAsync(activityId);
        return activity != null ? _mapper.MapActivityToDto(activity) : null;
    }

    public async Task<IEnumerable<ActivityDto>> GetDestinationActivitiesAsync(int destinationId)
    {
        var activities = destinationId > 0
            ? await _unitOfWork.Activities.GetDestinationActivitiesAsync(destinationId)
            : await _unitOfWork.Activities.GetAllAsync();

        return activities.Select(_mapper.MapActivityToDto);
    }

    public async Task<ActivityDto> CreateActivityAsync(int destinationId, CreateActivityDto dto)
    {
        var activity = _mapper.MapCreateActivityDtoToActivity(dto, destinationId);
        var createdActivity = await _unitOfWork.Activities.AddAsync(activity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.MapActivityToDto(createdActivity);
    }

    public async Task<ActivityDto> UpdateActivityAsync(int activityId, UpdateActivityDto dto)
    {
        var activity = await _unitOfWork.Activities.GetByIdAsync(activityId)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Activity), activityId);

        activity.Update(
            dto.Name,
            dto.ScheduledDateTime,
            dto.Description,
            dto.DurationMinutes,
            dto.Location,
            dto.EstimatedCost);

        var updatedActivity = await _unitOfWork.Activities.UpdateAsync(activity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.MapActivityToDto(updatedActivity);
    }

    public async Task<bool> DeleteActivityAsync(int activityId)
    {
        var result = await _unitOfWork.Activities.DeleteAsync(activityId);
        if (result)
        {
            await _unitOfWork.SaveChangesAsync();
        }
        return result;
    }
}

public class BudgetService : IBudgetService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BudgetService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BudgetDto?> GetBudgetByIdAsync(int budgetId)
    {
        var budget = await _unitOfWork.Budgets.GetByIdAsync(budgetId);
        return budget != null ? _mapper.MapBudgetToDto(budget) : null;
    }

    public async Task<IEnumerable<BudgetDto>> GetTripBudgetsAsync(int tripId)
    {
        var budgets = tripId > 0
            ? await _unitOfWork.Budgets.GetTripBudgetsAsync(tripId)
            : await _unitOfWork.Budgets.GetAllAsync();

        return budgets.Select(_mapper.MapBudgetToDto);
    }

    public async Task<BudgetDto> CreateBudgetAsync(int tripId, CreateBudgetDto dto)
    {
        var budget = _mapper.MapCreateBudgetDtoToBudget(dto, tripId);
        var createdBudget = await _unitOfWork.Budgets.AddAsync(budget);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.MapBudgetToDto(createdBudget);
    }

    public async Task<BudgetDto> UpdateBudgetAsync(int budgetId, UpdateBudgetDto dto)
    {
        var budget = await _unitOfWork.Budgets.GetByIdAsync(budgetId)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Budget), budgetId);

        budget.Update(dto.Category, dto.PlannedAmount, dto.ActualAmount, dto.Notes);

        var updatedBudget = await _unitOfWork.Budgets.UpdateAsync(budget);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.MapBudgetToDto(updatedBudget);
    }

    public async Task<bool> DeleteBudgetAsync(int budgetId)
    {
        var result = await _unitOfWork.Budgets.DeleteAsync(budgetId);
        if (result)
        {
            await _unitOfWork.SaveChangesAsync();
        }
        return result;
    }
}
