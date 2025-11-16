using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Repository spécifique pour les trips
/// </summary>
public interface ITripRepository : IRepository<Trip>
{
    /// <summary>
    /// Récupère tous les trips d'un convoi
    /// </summary>
    Task<IEnumerable<Trip>> GetConvoyTripsAsync(Guid convoyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le trip actif d'un convoi (s'il existe)
    /// </summary>
    Task<Trip?> GetActiveConvoyTripAsync(Guid convoyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si un convoi a un trip actif
    /// </summary>
    Task<bool> HasActiveTripAsync(Guid convoyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les trips avec leurs waypoints
    /// </summary>
    Task<Trip?> GetTripWithWaypointsAsync(Guid tripId, CancellationToken cancellationToken = default);
}
