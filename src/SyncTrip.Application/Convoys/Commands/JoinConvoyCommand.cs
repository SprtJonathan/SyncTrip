using MediatR;

namespace SyncTrip.Application.Convoys.Commands;

/// <summary>
/// Command pour rejoindre un convoi existant.
/// </summary>
public record JoinConvoyCommand : IRequest
{
    /// <summary>
    /// Code d'accès du convoi.
    /// </summary>
    public string JoinCode { get; init; } = string.Empty;

    /// <summary>
    /// Identifiant de l'utilisateur qui rejoint.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Identifiant du véhicule utilisé.
    /// </summary>
    public Guid VehicleId { get; init; }
}
