using MediatR;

namespace SyncTrip.Application.Convoys.Commands;

/// <summary>
/// Command pour quitter un convoi.
/// </summary>
public record LeaveConvoyCommand : IRequest
{
    /// <summary>
    /// Code d'accès du convoi.
    /// </summary>
    public string JoinCode { get; init; } = string.Empty;

    /// <summary>
    /// Identifiant de l'utilisateur qui quitte.
    /// </summary>
    public Guid UserId { get; init; }
}
