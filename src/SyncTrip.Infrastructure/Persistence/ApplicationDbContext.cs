using Microsoft.EntityFrameworkCore;
using SyncTrip.Core.Entities;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Appliquer toutes les configurations depuis l'assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
