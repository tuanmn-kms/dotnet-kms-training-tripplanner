using KMSTraining.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KMSTraining.Tests.Helpers;

public static class TestDbContextFactory
{
    public static TripPlannerDbContext CreateInMemoryContext(string databaseName = "")
    {
        var dbName = string.IsNullOrEmpty(databaseName) ? Guid.NewGuid().ToString() : databaseName;

        var options = new DbContextOptionsBuilder<TripPlannerDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new TripPlannerDbContext(options);
        return context;
    }
}
