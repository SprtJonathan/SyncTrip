using MediatR;
using SyncTrip.Core.Enums;
using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.Application.Trips.Commands;

/// <summary>
/// Command pour démarrer un nouveau voyage.
/// </summary>
public record StartTripCommand : IRequest<Guid>
{
    /// <summary>
    /// Identifiant du convoi.
    /// </summary>
    public Guid ConvoyId { get; init; }

    /// <summary>
    /// Identifiant de l'utilisateur (leader).
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Statut initial du voyage.
    /// </summary>
    public TripStatus Status { get; init; }

    /// <summary>
    /// Profil de route.
    /// </summary>
    public RouteProfile RouteProfile { get; init; }

    /// <summary>
    /// Waypoints initiaux (optionnels).
    /// </summary>
    public List<CreateWaypointRequest> Waypoints { get; init; } = new();
}
