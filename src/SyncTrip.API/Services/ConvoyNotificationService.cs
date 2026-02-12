using Microsoft.AspNetCore.SignalR;
using SyncTrip.API.Hubs;
using SyncTrip.Application.Chat.Services;
using SyncTrip.Shared.DTOs.Chat;

namespace SyncTrip.API.Services;

/// <summary>
/// Implémentation des notifications temps réel via SignalR pour le chat de convoi.
/// </summary>
public class ConvoyNotificationService : IConvoyNotificationService
{
    private readonly IHubContext<ConvoyHub> _hubContext;
    private readonly ILogger<ConvoyNotificationService> _logger;

    public ConvoyNotificationService(
        IHubContext<ConvoyHub> hubContext,
        ILogger<ConvoyNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyNewMessageAsync(Guid convoyId, MessageDto message, CancellationToken ct = default)
    {
        var groupName = $"convoy-{convoyId}";
        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", message, ct);
        _logger.LogInformation("Notification ReceiveMessage envoyée au groupe {Group}", groupName);
    }
}
