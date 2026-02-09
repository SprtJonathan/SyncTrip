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

        // Services
        services.AddScoped<IAuthService, AuthService>();

        if (environment.IsDevelopment())
            services.AddScoped<IEmailService, DevelopmentEmailService>();
        else
            services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
