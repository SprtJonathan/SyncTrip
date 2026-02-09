using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncTrip.Core.Entities;

namespace SyncTrip.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité Convoy.
/// </summary>
public class ConvoyConfiguration : IEntityTypeConfiguration<Convoy>
{
    public void Configure(EntityTypeBuilder<Convoy> builder)
    {
        builder.ToTable("Convoys");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.JoinCode)
            .IsRequired()
            .HasMaxLength(6);

        builder.Property(c => c.LeaderUserId)
            .IsRequired();

        builder.Property(c => c.IsPrivate)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        // Index unique sur le code d'accès
        builder.HasIndex(c => c.JoinCode)
            .IsUnique();

        // Index pour recherche par leader
        builder.HasIndex(c => c.LeaderUserId);
    }
}
