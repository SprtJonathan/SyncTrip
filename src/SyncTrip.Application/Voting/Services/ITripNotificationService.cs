using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.Application.Voting.Services;

/// <summary>
/// Interface pour les notifications temps réel liées aux voyages (vote, propositions).
/// </summary>
public interface ITripNotificationService
{
    /// <summary>
    /// Notifie les membres du voyage qu'une nouvelle proposition d'arrêt a été soumise.
    /// </summary>
    Task NotifyStopProposedAsync(Guid tripId, StopProposalDto proposal, CancellationToken ct = default);

    /// <summary>
    /// Notifie les membres du voyage d'une mise à jour des votes.
    /// </summary>
    Task NotifyVoteUpdateAsync(Guid tripId, Guid proposalId, int yesCount, int noCount, CancellationToken ct = default);

    /// <summary>
    /// Notifie les membres du voyage qu'une proposition a été résolue.
    /// </summary>
    Task NotifyProposalResolvedAsync(Guid tripId, StopProposalDto proposal, CancellationToken ct = default);
}
