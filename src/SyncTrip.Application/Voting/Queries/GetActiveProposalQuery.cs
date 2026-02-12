using MediatR;
using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.Application.Voting.Queries;

/// <summary>
/// Query pour récupérer la proposition active d'un voyage.
/// </summary>
public record GetActiveProposalQuery : IRequest<StopProposalDto?>
{
    /// <summary>
    /// Identifiant du voyage.
    /// </summary>
    public Guid TripId { get; init; }

    public GetActiveProposalQuery(Guid tripId)
    {
        TripId = tripId;
    }
}
