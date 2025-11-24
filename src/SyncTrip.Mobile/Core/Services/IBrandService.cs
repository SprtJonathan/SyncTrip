using SyncTrip.Shared.DTOs.Brands;

namespace SyncTrip.Mobile.Core.Services;

/// <summary>
/// Service de gestion des marques de véhicules.
/// Permet de récupérer la liste des marques disponibles.
/// </summary>
public interface IBrandService
{
    /// <summary>
    /// Récupère la liste de toutes les marques disponibles.
    /// </summary>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>Liste des marques ou liste vide en cas d'échec.</returns>
    Task<List<BrandDto>> GetBrandsAsync(CancellationToken ct = default);

    /// <summary>
    /// Récupère une marque spécifique par son identifiant.
    /// </summary>
    /// <param name="brandId">Identifiant de la marque.</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>Marque ou null si introuvable.</returns>
    Task<BrandDto?> GetBrandByIdAsync(int brandId, CancellationToken ct = default);
}
