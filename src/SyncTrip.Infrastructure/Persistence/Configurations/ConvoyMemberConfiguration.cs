using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;

namespace SyncTrip.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité ConvoyMember.
/// Table de liaison entre Convoy et User.
/// </summary>
public class ConvoyMemberConfiguration : IEntityTypeConfiguration<ConvoyMember>
{
    public void Configure(EntityTypeBuilder<ConvoyMember> builder)
    {
        builder.ToTable("ConvoyMembers");

        // Clé composite (ConvoyId, UserId)
        builder.HasKey(cm => new { cm.ConvoyId, cm.UserId });

        builder.Property(cm => cm.ConvoyId)
            .IsRequired();

        builder.Property(cm => cm.UserId)
            .IsRequired();

        builder.Property(cm => cm.VehicleId)
            .IsRequired();

        builder.Property(cm => cm.Role)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(cm => cm.JoinedAt)
            .IsRequired();

        // Relation many-to-one avec Convoy
        builder.HasOne(cm => cm.Convoy)
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.ConvoyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation many-to-one avec User
        builder.HasOne(cm => cm.User)
            .WithMany()
            .HasForeignKey(cm => cm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relation many-to-one avec Vehicle
        builder.HasOne(cm => cm.Vehicle)
            .WithMany()
            .HasForeignKey(cm => cm.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index pour recherche par utilisateur
        builder.HasIndex(cm => cm.UserId);
    }
}
