using SyncTrip.Core.Entities;

namespace SyncTrip.Core.Interfaces;

/// <summary>
/// Interface du repository pour les voyages (trips).
/// </summary>
public interface ITripRepository
{
    /// <summary>
    /// Récupère un voyage par son identifiant, avec ses waypoints et le convoi.
    /// </summary>
    Task<Trip?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Récupère le voyage actif (non terminé) d'un convoi.
    /// </summary>
    Task<Trip?> GetActiveByConvoyIdAsync(Guid convoyId, CancellationToken ct = default);

    /// <summary>
    /// Récupère tous les voyages d'un convoi.
    /// </summary>
    Task<IList<Trip>> GetByConvoyIdAsync(Guid convoyId, CancellationToken ct = default);

    /// <summary>
    /// Ajoute un nouveau voyage.
    /// </summary>
    Task AddAsync(Trip trip, CancellationToken ct = default);

    /// <summary>
    /// Met à jour un voyage existant.
    /// </summary>
    Task UpdateAsync(Trip trip, CancellationToken ct = default);
}
