using SyncTrip.Api.Application.DTOs.Locations;

namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Service de gestion des positions GPS
/// </summary>
public interface ILocationService
{
    /// <summary>
    /// Met à jour la position d'un utilisateur pour un trip
    /// </summary>
    Task<LocationDto> UpdateLocationAsync(Guid userId, Guid tripId, UpdateLocationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère la dernière position d'un utilisateur pour un trip
    /// </summary>
    Task<LocationDto?> GetLastUserLocationAsync(Guid userId, Guid tripId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les dernières positions de tous les participants d'un trip
    /// </summary>
    Task<IEnumerable<LocationDto>> GetTripParticipantsLocationsAsync(Guid tripId, CancellationToken cancellationToken = default);
}
