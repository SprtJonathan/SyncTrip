using MediatR;

namespace SyncTrip.Application.Convoys.Commands;

/// <summary>
/// Command pour transférer le leadership à un autre membre.
/// </summary>
public record TransferLeadershipCommand : IRequest
{
    /// <summary>
    /// Code d'accès du convoi.
    /// </summary>
    public string JoinCode { get; init; } = string.Empty;

    /// <summary>
    /// Identifiant du leader actuel.
    /// </summary>
    public Guid RequestingUserId { get; init; }

    /// <summary>
    /// Identifiant du nouveau leader.
    /// </summary>
    public Guid NewLeaderUserId { get; init; }
}
