using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;

namespace SyncTrip.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité UserLicense.
/// Table de liaison entre User et LicenseType.
/// </summary>
public class UserLicenseConfiguration : IEntityTypeConfiguration<UserLicense>
{
    public void Configure(EntityTypeBuilder<UserLicense> builder)
    {
        builder.ToTable("UserLicenses");

        // Clé composite (UserId, LicenseType)
        builder.HasKey(ul => new { ul.UserId, ul.LicenseType });

        builder.Property(ul => ul.UserId)
            .IsRequired();

        builder.Property(ul => ul.LicenseType)
            .IsRequired()
            .HasConversion<int>(); // Stocke l'enum en tant qu'int

        builder.Property(ul => ul.AddedAt)
            .IsRequired();

        // Relation many-to-one avec User
        builder.HasOne(ul => ul.User)
            .WithMany(u => u.Licenses)
            .HasForeignKey(ul => ul.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Supprimer les permis si l'utilisateur est supprimé

        // Index pour recherche par utilisateur
        builder.HasIndex(ul => ul.UserId);
    }
}
