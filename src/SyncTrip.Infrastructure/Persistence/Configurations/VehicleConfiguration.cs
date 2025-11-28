using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;

namespace SyncTrip.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité Vehicle.
/// </summary>
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.UserId)
            .IsRequired();

        builder.Property(v => v.BrandId)
            .IsRequired();

        builder.Property(v => v.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Type)
            .IsRequired()
            .HasConversion<int>(); // Stocke l'enum en tant qu'int

        builder.Property(v => v.Color)
            .HasMaxLength(50);

        builder.Property(v => v.Year);

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        // Relation many-to-one avec User
        builder.HasOne(v => v.User)
            .WithMany(u => u.Vehicles)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Supprimer les véhicules si l'utilisateur est supprimé

        // Relation many-to-one avec Brand (déjà configurée dans BrandConfiguration)
        builder.HasOne(v => v.Brand)
            .WithMany(b => b.Vehicles)
            .HasForeignKey(v => v.BrandId);

        // Index pour recherche par utilisateur
        builder.HasIndex(v => v.UserId);

        // Index pour recherche par marque
        builder.HasIndex(v => v.BrandId);
    }
}
