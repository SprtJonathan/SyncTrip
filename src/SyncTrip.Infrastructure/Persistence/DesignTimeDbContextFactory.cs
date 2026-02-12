using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SyncTrip.Infrastructure.Persistence;

/// <summary>
/// Factory pour créer le DbContext au design-time (migrations EF Core).
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Configuration par défaut pour les migrations
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Utiliser une connection string par défaut
        optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=synctrip;Username=postgres;Password=synctrip_dev_2026");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
