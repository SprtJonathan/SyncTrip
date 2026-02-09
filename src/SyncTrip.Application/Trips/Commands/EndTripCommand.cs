using MediatR;

namespace SyncTrip.Application.Trips.Commands;

/// <summary>
/// Command pour terminer un voyage.
/// </summary>
public record EndTripCommand : IRequest
{
    /// <summary>
    /// Identifiant du voyage.
    /// </summary>
    public Guid TripId { get; init; }

    /// <summary>
    /// Identifiant de l'utilisateur (leader).
    /// </summary>
    public Guid UserId { get; init; }
}
