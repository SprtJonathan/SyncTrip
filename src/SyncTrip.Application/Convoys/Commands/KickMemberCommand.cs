using MediatR;

namespace SyncTrip.Application.Convoys.Commands;

/// <summary>
/// Command pour exclure un membre du convoi (leader uniquement).
/// </summary>
public record KickMemberCommand : IRequest
{
    /// <summary>
    /// Code d'accès du convoi.
    /// </summary>
    public string JoinCode { get; init; } = string.Empty;

    /// <summary>
    /// Identifiant du leader faisant la demande.
    /// </summary>
    public Guid RequestingUserId { get; init; }

    /// <summary>
    /// Identifiant du membre à exclure.
    /// </summary>
    public Guid TargetUserId { get; init; }
}
