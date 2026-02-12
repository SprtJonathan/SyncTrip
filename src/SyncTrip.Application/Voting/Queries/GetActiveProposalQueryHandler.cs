using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.Application.Voting.Queries;

/// <summary>
/// Handler pour récupérer la proposition active d'un voyage.
/// </summary>
public class GetActiveProposalQueryHandler : IRequestHandler<GetActiveProposalQuery, StopProposalDto?>
{
    private readonly IStopProposalRepository _proposalRepository;
    private readonly ILogger<GetActiveProposalQueryHandler> _logger;

    public GetActiveProposalQueryHandler(
        IStopProposalRepository proposalRepository,
        ILogger<GetActiveProposalQueryHandler> logger)
    {
        _proposalRepository = proposalRepository;
        _logger = logger;
    }

    public async Task<StopProposalDto?> Handle(GetActiveProposalQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Récupération de la proposition active du voyage {TripId}", request.TripId);

        var proposal = await _proposalRepository.GetPendingByTripIdAsync(request.TripId, cancellationToken);
        if (proposal == null)
            return null;

        return new StopProposalDto
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
        };
    }
}
