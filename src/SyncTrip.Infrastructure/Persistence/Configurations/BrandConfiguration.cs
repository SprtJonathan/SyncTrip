using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncTrip.Core.Entities;

namespace SyncTrip.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité Brand.
/// </summary>
public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .ValueGeneratedNever(); // IDs définis manuellement pour le seed data

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.LogoUrl)
            .IsRequired()
            .HasMaxLength(500);

        // Index sur le nom pour recherche rapide
        builder.HasIndex(b => b.Name)
            .IsUnique();

        // Relation one-to-many avec Vehicle
        builder.HasMany(b => b.Vehicles)
            .WithOne(v => v.Brand)
            .HasForeignKey(v => v.BrandId)
            .OnDelete(DeleteBehavior.Restrict); // Empêcher la suppression si des véhicules existent
    }
}
