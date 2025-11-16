using SyncTrip.Api.Core.Entities;

namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Repository spécifique pour l'historique de localisation
/// </summary>
public interface ILocationHistoryRepository : IRepository<LocationHistory>
{
    /// <summary>
    /// Récupère la dernière position d'un utilisateur pour un trip
    /// </summary>
    Task<LocationHistory?> GetLastUserLocationAsync(Guid userId, Guid tripId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les dernières positions de tous les participants d'un trip
    /// </summary>
    Task<IEnumerable<LocationHistory>> GetTripParticipantsLastLocationsAsync(Guid tripId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime l'historique plus ancien que X jours
    /// </summary>
    Task DeleteOldHistoryAsync(int daysOld, CancellationToken cancellationToken = default);
}
