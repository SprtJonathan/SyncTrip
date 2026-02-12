using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Application.Voting.Services;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Exceptions;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.Application.Voting.Commands;

/// <summary>
/// Handler pour la commande de proposition d'arrêt.
/// </summary>
public class ProposeStopCommandHandler : IRequestHandler<ProposeStopCommand, Guid>
{
    private readonly ITripRepository _tripRepository;
    private readonly IStopProposalRepository _proposalRepository;
    private readonly ITripNotificationService _notificationService;
    private readonly ILogger<ProposeStopCommandHandler> _logger;

    public ProposeStopCommandHandler(
        ITripRepository tripRepository,
        IStopProposalRepository proposalRepository,
        ITripNotificationService notificationService,
        ILogger<ProposeStopCommandHandler> logger)
    {
        _tripRepository = tripRepository;
        _proposalRepository = proposalRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Guid> Handle(ProposeStopCommand request, CancellationToken cancellationToken)
    {
        // Récupérer le voyage avec le convoi et ses membres
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);
        if (trip == null)
            throw new KeyNotFoundException($"Voyage avec l'ID {request.TripId} introuvable.");

        // Vérifier que le voyage n'est pas terminé
        if (trip.Status == Core.Enums.TripStatus.Finished)
            throw new DomainException("Impossible de proposer un arrêt sur un voyage terminé.");

        // Vérifier que l'utilisateur est membre du convoi
        if (!trip.Convoy.IsMember(request.UserId))
            throw new UnauthorizedAccessException("Vous n'êtes pas membre de ce convoi.");

        // Vérifier qu'il n'y a pas déjà une proposition en attente
        var existingProposal = await _proposalRepository.GetPendingByTripIdAsync(request.TripId, cancellationToken);
        if (existingProposal != null)
            throw new InvalidOperationException("Une proposition d'arrêt est déjà en cours pour ce voyage.");

        // Créer la proposition
        var proposal = StopProposal.Create(
            request.TripId,
            request.UserId,
            request.StopType,
            request.Latitude,
            request.Longitude,
            request.LocationName);

        // Auto-vote OUI du proposeur
        proposal.CastVote(request.UserId, true);

        await _proposalRepository.AddAsync(proposal, cancellationToken);

        // Notifier les membres via SignalR
        var proposalDto = MapToDto(proposal);
        await _notificationService.NotifyStopProposedAsync(request.TripId, proposalDto, cancellationToken);

        _logger.LogInformation("Proposition d'arrêt {ProposalId} créée pour le voyage {TripId} par {UserId}",
            proposal.Id, request.TripId, request.UserId);

        return proposal.Id;
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
