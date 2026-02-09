using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncTrip.Core.Entities;

namespace SyncTrip.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité TripWaypoint.
/// </summary>
public class TripWaypointConfiguration : IEntityTypeConfiguration<TripWaypoint>
{
    public void Configure(EntityTypeBuilder<TripWaypoint> builder)
    {
        builder.ToTable("TripWaypoints");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.TripId)
            .IsRequired();

        builder.Property(w => w.OrderIndex)
            .IsRequired();

        builder.Property(w => w.Latitude)
            .IsRequired()
            .HasPrecision(10, 7);

        builder.Property(w => w.Longitude)
            .IsRequired()
            .HasPrecision(10, 7);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(w => w.AddedByUserId)
            .IsRequired();

        // Relation many-to-one avec Trip
        builder.HasOne(w => w.Trip)
            .WithMany(t => t.Waypoints)
            .HasForeignKey(w => w.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation many-to-one avec User
        builder.HasOne(w => w.AddedByUser)
            .WithMany()
            .HasForeignKey(w => w.AddedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index composite pour recherche par voyage et ordre
        builder.HasIndex(w => new { w.TripId, w.OrderIndex });
    }
}
