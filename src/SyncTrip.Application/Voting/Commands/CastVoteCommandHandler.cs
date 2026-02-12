using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Application.Voting.Services;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.Application.Voting.Commands;

/// <summary>
/// Handler pour la commande de vote sur une proposition d'arrêt.
/// </summary>
public class CastVoteCommandHandler : IRequestHandler<CastVoteCommand>
{
    private readonly IStopProposalRepository _proposalRepository;
    private readonly ITripRepository _tripRepository;
    private readonly ITripNotificationService _notificationService;
    private readonly ILogger<CastVoteCommandHandler> _logger;

    public CastVoteCommandHandler(
        IStopProposalRepository proposalRepository,
        ITripRepository tripRepository,
        ITripNotificationService notificationService,
        ILogger<CastVoteCommandHandler> logger)
    {
        _proposalRepository = proposalRepository;
        _tripRepository = tripRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(CastVoteCommand request, CancellationToken cancellationToken)
    {
        // Récupérer la proposition avec ses votes, le voyage et les membres du convoi
        var proposal = await _proposalRepository.GetByIdAsync(request.ProposalId, cancellationToken);
        if (proposal == null)
            throw new KeyNotFoundException($"Proposition avec l'ID {request.ProposalId} introuvable.");

        // Vérifier que l'utilisateur est membre du convoi
        if (!proposal.Trip.Convoy.IsMember(request.UserId))
            throw new UnauthorizedAccessException("Vous n'êtes pas membre de ce convoi.");

        // Enregistrer le vote (la méthode domain valide le statut et le doublon)
        proposal.CastVote(request.UserId, request.IsYes);

        await _proposalRepository.UpdateAsync(proposal, cancellationToken);

        // Notifier la mise à jour des votes
        var yesCount = proposal.Votes.Count(v => v.IsYes);
        var noCount = proposal.Votes.Count(v => !v.IsYes);
        await _notificationService.NotifyVoteUpdateAsync(
            proposal.TripId, proposal.Id, yesCount, noCount, cancellationToken);

        _logger.LogInformation("Vote enregistré sur la proposition {ProposalId} par {UserId} (IsYes={IsYes})",
            request.ProposalId, request.UserId, request.IsYes);

        // Résolution anticipée si tous les membres ont voté
        var totalMembers = proposal.Trip.Convoy.Members.Count;
        if (proposal.AllMembersVoted(totalMembers))
        {
            proposal.Resolve(totalMembers);

            // Si accepté, créer le waypoint automatiquement
            if (proposal.Status == ProposalStatus.Accepted)
            {
                var trip = proposal.Trip;
                var waypointOrder = trip.Waypoints.Count;
                var waypoint = trip.AddWaypoint(
                    waypointOrder,
                    proposal.Latitude,
                    proposal.Longitude,
                    proposal.LocationName,
                    WaypointType.Stopover,
                    proposal.ProposedByUserId);

                proposal.SetCreatedWaypoint(waypoint.Id);
                await _tripRepository.UpdateAsync(trip, cancellationToken);
            }

            await _proposalRepository.UpdateAsync(proposal, cancellationToken);

            // Notifier la résolution
            var proposalDto = MapToDto(proposal);
            await _notificationService.NotifyProposalResolvedAsync(
                proposal.TripId, proposalDto, cancellationToken);

            _logger.LogInformation("Proposition {ProposalId} résolue anticipément : {Status}",
                proposal.Id, proposal.Status);
        }
    }

    private static StopProposalDto MapToDto(StopProposal proposal)
    {
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
