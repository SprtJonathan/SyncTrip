using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SyncTrip.Application.Voting.Services;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.Infrastructure.Services;

/// <summary>
/// Service d'arrière-plan qui résout les propositions d'arrêt expirées.
/// Poll toutes les 5 secondes.
/// </summary>
public class ProposalResolutionService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ProposalResolutionService> _logger;
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);

    public ProposalResolutionService(
        IServiceScopeFactory scopeFactory,
        ILogger<ProposalResolutionService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ProposalResolutionService démarré.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ResolveExpiredProposalsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la résolution des propositions expirées.");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }

        _logger.LogInformation("ProposalResolutionService arrêté.");
    }

    private async Task ResolveExpiredProposalsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var proposalRepository = scope.ServiceProvider.GetRequiredService<IStopProposalRepository>();
        var tripRepository = scope.ServiceProvider.GetRequiredService<ITripRepository>();
        var notificationService = scope.ServiceProvider.GetRequiredService<ITripNotificationService>();

        var expiredProposals = await proposalRepository.GetExpiredPendingAsync(ct);

        foreach (var proposal in expiredProposals)
        {
            try
            {
                var totalMembers = proposal.Trip.Convoy.Members.Count;
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
                    await tripRepository.UpdateAsync(trip, ct);
                }

                await proposalRepository.UpdateAsync(proposal, ct);

                // Notifier la résolution
                var proposalDto = new StopProposalDto
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

                await notificationService.NotifyProposalResolvedAsync(proposal.TripId, proposalDto, ct);

                _logger.LogInformation("Proposition {ProposalId} résolue par timer : {Status}",
                    proposal.Id, proposal.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la résolution de la proposition {ProposalId}", proposal.Id);
            }
        }
    }
}
