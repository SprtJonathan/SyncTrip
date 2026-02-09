using MediatR;

namespace SyncTrip.Application.Convoys.Commands;

/// <summary>
/// Command pour dissoudre un convoi (leader uniquement).
/// </summary>
public record DissolveConvoyCommand : IRequest
{
    /// <summary>
    /// Code d'accès du convoi.
    /// </summary>
    public string JoinCode { get; init; } = string.Empty;

    /// <summary>
    /// Identifiant du leader faisant la demande.
    /// </summary>
    public Guid RequestingUserId { get; init; }
}
