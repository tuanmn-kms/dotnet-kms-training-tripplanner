using KMSTraining.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace KMSTraining.API.Data;

public class TripPlannerDbContext : DbContext
{
    public TripPlannerDbContext(DbContextOptions<TripPlannerDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<Destination> Destinations { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<Budget> Budgets { get; set; }

    public override int SaveChanges()
    {
        NormalizeDateTimesToUtc();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        NormalizeDateTimesToUtc();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        NormalizeDateTimesToUtc();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        NormalizeDateTimesToUtc();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasOne(t => t.User)
                .WithMany(u => u.Trips)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Destination>(entity =>
        {
            entity.HasOne(d => d.Trip)
                .WithMany(t => t.Destinations)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasOne(a => a.Destination)
                .WithMany(d => d.Activities)
                .HasForeignKey(a => a.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasOne(b => b.Trip)
                .WithMany(t => t.Budgets)
                .HasForeignKey(b => b.TripId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void NormalizeDateTimesToUtc()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            if (entity is null)
            {
                continue;
            }

            var properties = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(DateTime))
                {
                    var value = (DateTime)property.GetValue(entity)!;
                    property.SetValue(entity, NormalizeToUtc(value));
                    continue;
                }

                if (property.PropertyType == typeof(DateTime?))
                {
                    var value = (DateTime?)property.GetValue(entity);
                    if (value.HasValue)
                    {
                        property.SetValue(entity, NormalizeToUtc(value.Value));
                    }
                }
            }
        }
    }

    private static DateTime NormalizeToUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
