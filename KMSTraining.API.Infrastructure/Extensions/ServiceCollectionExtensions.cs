using KMSTraining.API.Application.Mapping;
using KMSTraining.API.Application.Services;
using KMSTraining.API.Domain.Interfaces;
using KMSTraining.API.Infrastructure.Data;
using KMSTraining.API.Infrastructure.Services;
using KMSTraining.API.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KMSTraining.API.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds infrastructure services including DbContext, repositories, and Unit of Work
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add DbContext
        var databaseProvider = (configuration["DatabaseProvider"] ?? "sqlserver").Trim().ToLowerInvariant();
        var connectionString = configuration.GetConnectionString("TripPlannerDatabase")
            ?? throw new InvalidOperationException("Connection string 'TripPlannerDatabase' is not configured.");

        services.AddDbContext<TripPlannerDbContext>(options =>
        {
            if (databaseProvider is "postgres" or "postgresql")
            {
                options.UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.CommandTimeout(60);
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorCodesToAdd: null);
                    });
            }
            else
            {
                options.UseSqlServer(
                    connectionString,
                    sqlOptions =>
                    {
                        sqlOptions.CommandTimeout(60);
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    });
            }
        });

        // Add Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        // Add Token Service
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }

    /// <summary>
    /// Adds application services including mappers and business logic services
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add Mapper
        services.AddScoped<IMapper, Mapper>();

        // Add Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITripService, TripService>();
        services.AddScoped<IDestinationService, DestinationService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IBudgetService, BudgetService>();

        return services;
    }
}
