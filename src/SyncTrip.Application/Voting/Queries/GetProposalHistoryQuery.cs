using MediatR;
using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.Application.Voting.Queries;

/// <summary>
/// Query pour récupérer l'historique des propositions d'un voyage.
/// </summary>
public record GetProposalHistoryQuery : IRequest<IList<StopProposalDto>>
{
    /// <summary>
    /// Identifiant du voyage.
    /// </summary>
    public Guid TripId { get; init; }

    public GetProposalHistoryQuery(Guid tripId)
    {
        TripId = tripId;
    }
}
