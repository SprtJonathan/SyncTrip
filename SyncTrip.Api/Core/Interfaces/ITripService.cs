using SyncTrip.Api.Application.DTOs.Trips;
using SyncTrip.Api.Application.DTOs.Waypoints;

namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Service de gestion des trips
/// </summary>
public interface ITripService
{
    /// <summary>
    /// Crée un nouveau trip
    /// </summary>
    Task<TripDto> CreateTripAsync(Guid userId, CreateTripRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un trip par son ID
    /// </summary>
    Task<TripDto?> GetTripByIdAsync(Guid tripId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère tous les trips d'un convoi
    /// </summary>
    Task<IEnumerable<TripDto>> GetConvoyTripsAsync(Guid convoyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le trip actif d'un convoi
    /// </summary>
    Task<TripDto?> GetActiveConvoyTripAsync(Guid convoyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour le statut d'un trip
    /// </summary>
    Task<TripDto> UpdateTripStatusAsync(Guid tripId, UpdateTripStatusRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ajoute un waypoint à un trip
    /// </summary>
    Task<WaypointDto> AddWaypointAsync(Guid tripId, CreateWaypointRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marque un waypoint comme atteint
    /// </summary>
    Task MarkWaypointReachedAsync(Guid waypointId, CancellationToken cancellationToken = default);
}
