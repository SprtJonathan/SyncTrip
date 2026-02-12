using Microsoft.AspNetCore.SignalR;
using SyncTrip.API.Hubs;
using SyncTrip.Application.Voting.Services;
using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.API.Services;

/// <summary>
/// Implémentation des notifications temps réel via SignalR pour les votes.
/// </summary>
public class TripNotificationService : ITripNotificationService
{
    private readonly IHubContext<TripHub> _hubContext;
    private readonly ILogger<TripNotificationService> _logger;

    public TripNotificationService(
        IHubContext<TripHub> hubContext,
        ILogger<TripNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyStopProposedAsync(Guid tripId, StopProposalDto proposal, CancellationToken ct = default)
    {
        var groupName = $"trip-{tripId}";
        await _hubContext.Clients.Group(groupName).SendAsync("StopProposed", proposal, ct);
        _logger.LogInformation("Notification StopProposed envoyée au groupe {Group}", groupName);
    }

    public async Task NotifyVoteUpdateAsync(Guid tripId, Guid proposalId, int yesCount, int noCount, CancellationToken ct = default)
    {
        var groupName = $"trip-{tripId}";
        await _hubContext.Clients.Group(groupName).SendAsync("VoteUpdate", new
        {
            ProposalId = proposalId,
            YesCount = yesCount,
            NoCount = noCount
        }, ct);
        _logger.LogInformation("Notification VoteUpdate envoyée au groupe {Group}", groupName);
    }

    public async Task NotifyProposalResolvedAsync(Guid tripId, StopProposalDto proposal, CancellationToken ct = default)
    {
        var groupName = $"trip-{tripId}";
        await _hubContext.Clients.Group(groupName).SendAsync("ProposalResolved", proposal, ct);
        _logger.LogInformation("Notification ProposalResolved envoyée au groupe {Group}", groupName);
    }
}
