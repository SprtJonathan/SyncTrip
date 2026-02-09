using MediatR;

namespace SyncTrip.Application.Trips.Commands;

/// <summary>
/// Command pour supprimer un waypoint d'un voyage.
/// </summary>
public record RemoveWaypointCommand : IRequest
{
    /// <summary>
    /// Identifiant du voyage.
    /// </summary>
    public Guid TripId { get; init; }

    /// <summary>
    /// Identifiant du waypoint à supprimer.
    /// </summary>
    public Guid WaypointId { get; init; }

    /// <summary>
    /// Identifiant de l'utilisateur (leader).
    /// </summary>
    public Guid UserId { get; init; }
}
