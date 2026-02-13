using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncTrip.Core.Entities;

namespace SyncTrip.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité Trip.
/// </summary>
public class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.ToTable("Trips");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.ConvoyId)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.StartTime)
            .IsRequired();

        builder.Property(t => t.EndTime);

        builder.Property(t => t.RouteProfile)
            .IsRequired()
            .HasConversion<int>();

        // Relation many-to-one avec Convoy
        builder.HasOne(t => t.Convoy)
            .WithMany(c => c.Trips)
            .HasForeignKey(t => t.ConvoyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Route geometry (GeoJSON)
        builder.Property(t => t.RouteGeometry)
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(t => t.RouteDistanceMeters)
            .IsRequired(false);

        builder.Property(t => t.RouteDurationSeconds)
            .IsRequired(false);

        // Index composite pour recherche par convoi et statut
        builder.HasIndex(t => new { t.ConvoyId, t.Status });
    }
}
