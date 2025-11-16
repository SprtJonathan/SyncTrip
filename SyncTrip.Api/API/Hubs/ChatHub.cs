using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SyncTrip.Api.Application.DTOs.Messages;
using SyncTrip.Api.Core.Interfaces;
using System.Security.Claims;

namespace SyncTrip.Api.API.Hubs;

/// <summary>
/// Hub SignalR pour le chat en temps réel
/// </summary>
[Authorize]
public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IMessageService messageService, ILogger<ChatHub> logger)
    {
        _messageService = messageService;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;
    }

    /// <summary>
    /// Rejoindre le chat d'un convoi
    /// </summary>
    public async Task JoinConvoyChat(Guid convoyId)
    {
        var userId = GetCurrentUserId();
        var groupName = $"convoy_{convoyId}";

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("User {UserId} joined convoy chat {ConvoyId}", userId, convoyId);

        // Charger les derniers messages
        var messages = await _messageService.GetConvoyMessagesAsync(convoyId, 0, 50);
        await Clients.Caller.SendAsync("ReceiveMessageHistory", messages);
    }

    /// <summary>
    /// Quitter le chat d'un convoi
    /// </summary>
    public async Task LeaveConvoyChat(Guid convoyId)
    {
        var groupName = $"convoy_{convoyId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("User left convoy chat {ConvoyId}", convoyId);
    }

    /// <summary>
    /// Envoyer un message dans le chat
    /// </summary>
    public async Task SendMessage(Guid convoyId, SendMessageRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var message = await _messageService.SendMessageAsync(userId, convoyId, request);

            // Diffuser le message à tous les membres du convoi
            var groupName = $"convoy_{convoyId}";
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message);

            _logger.LogInformation("Message sent in convoy {ConvoyId} by user {UserId}", convoyId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogError(exception, "Client disconnected with error");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
