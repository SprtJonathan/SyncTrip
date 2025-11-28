using Microsoft.EntityFrameworkCore;
using SyncTrip.Core.Entities;
using SyncTrip.Infrastructure.Persistence.Seed;

namespace SyncTrip.Infrastructure.Persistence;

/// <summary>
/// Contexte de base de données principal pour SyncTrip.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Table des utilisateurs.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Table des tokens Magic Link.
    /// </summary>
    public DbSet<MagicLinkToken> MagicLinkTokens => Set<MagicLinkToken>();

    /// <summary>
    /// Table des véhicules.
    /// </summary>
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    /// <summary>
    /// Table des marques de véhicules.
    /// </summary>
    public DbSet<Brand> Brands => Set<Brand>();

    /// <summary>
    /// Table des permis de conduire des utilisateurs.
    /// </summary>
    public DbSet<UserLicense> UserLicenses => Set<UserLicense>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Appliquer toutes les configurations depuis l'assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Appliquer le seed data
        modelBuilder.SeedBrands();
    }
}
