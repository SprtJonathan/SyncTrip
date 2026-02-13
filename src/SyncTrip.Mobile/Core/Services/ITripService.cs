using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.Mobile.Core.Services;

/// <summary>
/// Service de gestion des voyages GPS.
/// </summary>
public interface ITripService
{
    /// <summary>
    /// Démarre un nouveau voyage pour un convoi.
    /// </summary>
    Task<Guid?> StartTripAsync(Guid convoyId, StartTripRequest request, CancellationToken ct = default);

    /// <summary>
    /// Récupère le voyage actif d'un convoi.
    /// </summary>
    Task<TripDetailsDto?> GetActiveTripAsync(Guid convoyId, CancellationToken ct = default);

    /// <summary>
    /// Récupère un voyage par son identifiant.
    /// </summary>
    Task<TripDetailsDto?> GetTripByIdAsync(Guid convoyId, Guid tripId, CancellationToken ct = default);

    /// <summary>
    /// Liste l'historique des voyages d'un convoi.
    /// </summary>
    Task<List<TripDto>> GetConvoyTripsAsync(Guid convoyId, CancellationToken ct = default);

    /// <summary>
    /// Termine un voyage en cours.
    /// </summary>
    Task<bool> EndTripAsync(Guid convoyId, Guid tripId, CancellationToken ct = default);
}
