using KMSTraining.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace KMSTraining.API.Infrastructure.Data;

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

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasMany(u => u.Trips).WithOne(t => t.User).HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Trip configuration
        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.HasOne(t => t.User).WithMany(u => u.Trips).HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(t => t.Destinations).WithOne(d => d.Trip).HasForeignKey(d => d.TripId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(t => t.Budgets).WithOne(b => b.Trip).HasForeignKey(b => b.TripId).OnDelete(DeleteBehavior.Cascade);
        });

        // Destination configuration
        modelBuilder.Entity<Destination>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.ArrivalDate).IsRequired();
            entity.Property(e => e.DepartureDate).IsRequired();
            entity.HasOne(d => d.Trip).WithMany(t => t.Destinations).HasForeignKey(d => d.TripId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Activities).WithOne(a => a.Destination).HasForeignKey(a => a.DestinationId).OnDelete(DeleteBehavior.Cascade);
        });

        // Activity configuration
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ScheduledDateTime).IsRequired();
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.EstimatedCost).HasPrecision(18, 2);
            entity.HasOne(a => a.Destination).WithMany(d => d.Activities).HasForeignKey(a => a.DestinationId).OnDelete(DeleteBehavior.Cascade);
        });

        // Budget configuration
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PlannedAmount).HasPrecision(18, 2);
            entity.Property(e => e.ActualAmount).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.HasOne(b => b.Trip).WithMany(t => t.Budgets).HasForeignKey(b => b.TripId).OnDelete(DeleteBehavior.Cascade);
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
                if (property.PropertyType != typeof(DateTime) && property.PropertyType != typeof(DateTime?))
                {
                    continue;
                }

                var value = property.GetValue(entity);
                if (value is DateTime dateTime && dateTime.Kind != DateTimeKind.Utc)
                {
                    property.SetValue(entity, DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
                }
            }
        }
    }
}
