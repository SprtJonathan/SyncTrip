using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SyncTrip.API.Hubs;

/// <summary>
/// Hub SignalR pour la communication temps réel au niveau du convoi (chat).
/// </summary>
[Authorize]
public class ConvoyHub : Hub
{
    private readonly ILogger<ConvoyHub> _logger;

    public ConvoyHub(ILogger<ConvoyHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Rejoint le groupe d'un convoi pour recevoir les messages.
    /// </summary>
    /// <param name="convoyId">Identifiant du convoi.</param>
    public async Task JoinConvoy(Guid convoyId)
    {
        var groupName = $"convoy-{convoyId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Connexion {ConnectionId} a rejoint le groupe {Group}", Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Quitte le groupe d'un convoi.
    /// </summary>
    /// <param name="convoyId">Identifiant du convoi.</param>
    public async Task LeaveConvoy(Guid convoyId)
    {
        var groupName = $"convoy-{convoyId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Connexion {ConnectionId} a quitté le groupe {Group}", Context.ConnectionId, groupName);
    }
}
