using MediatR;

namespace SyncTrip.Application.Voting.Commands;

/// <summary>
/// Command pour voter sur une proposition d'arrêt.
/// </summary>
public record CastVoteCommand : IRequest
{
    /// <summary>
    /// Identifiant de la proposition.
    /// </summary>
    public Guid ProposalId { get; init; }

    /// <summary>
    /// Identifiant de l'utilisateur votant.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// True pour voter OUI, False pour voter NON.
    /// </summary>
    public bool IsYes { get; init; }
}
