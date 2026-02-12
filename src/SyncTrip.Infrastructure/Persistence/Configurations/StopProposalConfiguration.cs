using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncTrip.Core.Entities;

namespace SyncTrip.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité StopProposal.
/// </summary>
public class StopProposalConfiguration : IEntityTypeConfiguration<StopProposal>
{
    public void Configure(EntityTypeBuilder<StopProposal> builder)
    {
        builder.ToTable("StopProposals");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.TripId)
            .IsRequired();

        builder.Property(p => p.ProposedByUserId)
            .IsRequired();

        builder.Property(p => p.StopType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.Latitude)
            .IsRequired();

        builder.Property(p => p.Longitude)
            .IsRequired();

        builder.Property(p => p.LocationName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.ExpiresAt)
            .IsRequired();

        builder.Property(p => p.ResolvedAt);

        builder.Property(p => p.CreatedWaypointId);

        // Relation many-to-one avec Trip
        builder.HasOne(p => p.Trip)
            .WithMany()
            .HasForeignKey(p => p.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation many-to-one avec User (proposeur)
        builder.HasOne(p => p.ProposedByUser)
            .WithMany()
            .HasForeignKey(p => p.ProposedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index composite pour recherche par voyage et statut
        builder.HasIndex(p => new { p.TripId, p.Status });

        // Index pour le background service (propositions expirées)
        builder.HasIndex(p => new { p.Status, p.ExpiresAt });
    }
}
