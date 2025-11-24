using SyncTrip.Core.Entities;

namespace SyncTrip.Core.Interfaces;

/// <summary>
/// Interface du repository pour les marques de véhicules.
/// </summary>
public interface IBrandRepository
{
    /// <summary>
    /// Récupère une marque par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant de la marque.</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>La marque ou null si non trouvée.</returns>
    Task<Brand?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Récupère toutes les marques.
    /// </summary>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>Liste de toutes les marques.</returns>
    Task<IList<Brand>> GetAllAsync(CancellationToken ct = default);
}
