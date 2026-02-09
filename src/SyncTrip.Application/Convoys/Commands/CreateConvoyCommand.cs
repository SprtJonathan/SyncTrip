using MediatR;

namespace SyncTrip.Application.Convoys.Commands;

/// <summary>
/// Command pour créer un nouveau convoi.
/// </summary>
public record CreateConvoyCommand : IRequest<Guid>
{
    /// <summary>
    /// Identifiant de l'utilisateur créateur (leader).
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Identifiant du véhicule utilisé par le leader.
    /// </summary>
    public Guid VehicleId { get; init; }

    /// <summary>
    /// Indique si le convoi est privé.
    /// </summary>
    public bool IsPrivate { get; init; }
}
