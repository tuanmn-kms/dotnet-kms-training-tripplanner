using KMSTraining.API.Models;
using Microsoft.EntityFrameworkCore;

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
}
