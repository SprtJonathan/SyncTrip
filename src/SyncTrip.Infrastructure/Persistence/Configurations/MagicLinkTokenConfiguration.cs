using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncTrip.Core.Entities;

namespace SyncTrip.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité MagicLinkToken.
/// </summary>
public class MagicLinkTokenConfiguration : IEntityTypeConfiguration<MagicLinkToken>
{
    public void Configure(EntityTypeBuilder<MagicLinkToken> builder)
    {
        builder.ToTable("MagicLinkTokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever(); // Géré par le domain

        builder.Property(t => t.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(t => t.Email);

        builder.Property(t => t.Token)
            .IsRequired()
            .HasMaxLength(128); // SHA256 hex = 64 chars, mais on met 128 pour être sûr

        builder.HasIndex(t => t.Token);

        builder.Property(t => t.ExpiresAt)
            .IsRequired();

        builder.Property(t => t.UsedAt);

        builder.Property(t => t.CreatedAt)
            .IsRequired();
    }
}
