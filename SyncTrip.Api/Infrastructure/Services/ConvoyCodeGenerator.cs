using Microsoft.EntityFrameworkCore;
using SyncTrip.Api.Core.Interfaces;
using SyncTrip.Api.Infrastructure.Data;

namespace SyncTrip.Api.Infrastructure.Services;

/// <summary>
/// Générateur de codes uniques pour les convois
/// Codes de 6 caractères [A-Za-z0-9] = 62^6 = 56,800,235,584 possibilités
/// </summary>
public class ConvoyCodeGenerator : IConvoyCodeGenerator
{
    private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int CodeLength = 6;
    private const int MaxRetries = 10;

    private readonly ApplicationDbContext _context;
    private readonly Random _random;

    public ConvoyCodeGenerator(ApplicationDbContext context)
    {
        _context = context;
        _random = new Random();
    }

    /// <summary>
    /// Génère un code unique de 6 caractères alphanumériques
    /// </summary>
    /// <returns>Code unique de convoi</returns>
    /// <exception cref="InvalidOperationException">Si impossible de générer un code unique après plusieurs tentatives</exception>
    public async Task<string> GenerateUniqueCodeAsync()
    {
        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            var code = GenerateRandomCode();

            // Vérifier l'unicité dans la base de données
            var exists = await _context.Convoys
                .IgnoreQueryFilters() // Inclure les convois archivés/supprimés
                .AnyAsync(c => c.Code == code);

            if (!exists)
            {
                return code;
            }

            // Collision détectée (extrêmement rare avec 56 milliards de possibilités)
            // On réessaye avec un nouveau code
        }

        // Si on arrive ici, on a eu MaxRetries collisions consécutives (quasi impossible)
        throw new InvalidOperationException(
            $"Impossible de générer un code unique après {MaxRetries} tentatives. " +
            "Cela indique probablement un problème avec le générateur aléatoire ou la base de données.");
    }

    /// <summary>
    /// Génère un code aléatoire de 6 caractères
    /// </summary>
    private string GenerateRandomCode()
    {
        var code = new char[CodeLength];

        for (int i = 0; i < CodeLength; i++)
        {
            code[i] = Characters[_random.Next(Characters.Length)];
        }

        return new string(code);
    }
}
