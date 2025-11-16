namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Service de génération de codes uniques pour les convois
/// </summary>
public interface IConvoyCodeGenerator
{
    /// <summary>
    /// Génère un code unique de 6 caractères alphanumériques [A-Za-z0-9]
    /// </summary>
    /// <returns>Code unique de convoi</returns>
    Task<string> GenerateUniqueCodeAsync();
}
