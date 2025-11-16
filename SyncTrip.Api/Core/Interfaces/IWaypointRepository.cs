using SyncTrip.Api.Core.Entities;

namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Repository spécifique pour les waypoints
/// </summary>
public interface IWaypointRepository : IRepository<Waypoint>
{
    /// <summary>
    /// Récupère les waypoints d'un trip, ordonnés
    /// </summary>
    Task<IEnumerable<Waypoint>> GetTripWaypointsAsync(Guid tripId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les waypoints non atteints d'un trip
    /// </summary>
    Task<IEnumerable<Waypoint>> GetUnreachedWaypointsAsync(Guid tripId, CancellationToken cancellationToken = default);
}
