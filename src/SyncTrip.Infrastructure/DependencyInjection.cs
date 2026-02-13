using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SyncTrip.Core.Interfaces;
using SyncTrip.Infrastructure.Persistence;
using SyncTrip.Infrastructure.Repositories;
using SyncTrip.Infrastructure.Services;

namespace SyncTrip.Infrastructure;

/// <summary>
/// Configuration de l'injection de dépendances pour la couche Infrastructure.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Enregistre les services de la couche Infrastructure dans le conteneur DI.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Base de données PostgreSQL
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMagicLinkTokenRepository, MagicLinkTokenRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IConvoyRepository, ConvoyRepository>();
        services.AddScoped<ITripRepository, TripRepository>();
        services.AddScoped<IStopProposalRepository, StopProposalRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IEmailService, EmailService>();

        // External API services
        services.AddHttpClient<IGeocodingService, NominatimGeocodingService>(client =>
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("SyncTrip/1.0 (contact@synctrip.com)");
        });
        services.AddHttpClient<IRoutingService, OsrmRoutingService>(client =>
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("SyncTrip/1.0 (contact@synctrip.com)");
        });

        // Background Services
        services.AddHostedService<ProposalResolutionService>();

        return services;
    }
}
