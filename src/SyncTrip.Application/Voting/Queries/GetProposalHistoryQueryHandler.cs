using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.Application.Voting.Queries;

/// <summary>
/// Handler pour récupérer l'historique des propositions d'un voyage.
/// </summary>
public class GetProposalHistoryQueryHandler : IRequestHandler<GetProposalHistoryQuery, IList<StopProposalDto>>
{
    private readonly IStopProposalRepository _proposalRepository;
    private readonly ILogger<GetProposalHistoryQueryHandler> _logger;

    public GetProposalHistoryQueryHandler(
        IStopProposalRepository proposalRepository,
        ILogger<GetProposalHistoryQueryHandler> logger)
    {
        _proposalRepository = proposalRepository;
        _logger = logger;
    }

    public async Task<IList<StopProposalDto>> Handle(GetProposalHistoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Récupération de l'historique des propositions du voyage {TripId}", request.TripId);

        var proposals = await _proposalRepository.GetByTripIdAsync(request.TripId, cancellationToken);

        return proposals.Select(proposal => new StopProposalDto
        {
            Id = proposal.Id,
            TripId = proposal.TripId,
            ProposedByUserId = proposal.ProposedByUserId,
            ProposedByUsername = proposal.ProposedByUser?.Username ?? string.Empty,
            StopType = (int)proposal.StopType,
            Latitude = proposal.Latitude,
            Longitude = proposal.Longitude,
            LocationName = proposal.LocationName,
            Status = (int)proposal.Status,
            CreatedAt = proposal.CreatedAt,
            ExpiresAt = proposal.ExpiresAt,
            ResolvedAt = proposal.ResolvedAt,
            YesCount = proposal.Votes.Count(v => v.IsYes),
            NoCount = proposal.Votes.Count(v => !v.IsYes),
            CreatedWaypointId = proposal.CreatedWaypointId,
            Votes = proposal.Votes.Select(v => new VoteDto
            {
                Id = v.Id,
                UserId = v.UserId,
                Username = v.User?.Username ?? string.Empty,
                IsYes = v.IsYes,
                VotedAt = v.VotedAt
            }).ToList()
        }).ToList();
    }
}
