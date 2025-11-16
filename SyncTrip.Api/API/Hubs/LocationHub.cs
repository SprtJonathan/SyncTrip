using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SyncTrip.Api.Application.DTOs.Locations;
using SyncTrip.Api.Core.Interfaces;
using System.Security.Claims;

namespace SyncTrip.Api.API.Hubs;

/// <summary>
/// Hub SignalR pour le partage de positions GPS en temps réel
/// </summary>
[Authorize]
public class LocationHub : Hub
{
    private readonly ILocationService _locationService;
    private readonly ILogger<LocationHub> _logger;

    public LocationHub(ILocationService locationService, ILogger<LocationHub> logger)
    {
        _locationService = locationService;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;
    }

    /// <summary>
    /// Rejoindre la room d'un trip pour recevoir les mises à jour de position
    /// </summary>
    public async Task JoinTrip(Guid tripId)
    {
        var userId = GetCurrentUserId();
        var groupName = $"trip_{tripId}";

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("User {UserId} joined trip {TripId}", userId, tripId);

        // Envoyer les positions actuelles de tous les participants
        var locations = await _locationService.GetTripParticipantsLocationsAsync(tripId);
        await Clients.Caller.SendAsync("ReceiveAllLocations", locations);
    }

    /// <summary>
    /// Quitter la room d'un trip
    /// </summary>
    public async Task LeaveTrip(Guid tripId)
    {
        var groupName = $"trip_{tripId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("User left trip {TripId}", tripId);
    }

    /// <summary>
    /// Mettre à jour sa position GPS
    /// </summary>
    public async Task UpdateLocation(Guid tripId, UpdateLocationRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var location = await _locationService.UpdateLocationAsync(userId, tripId, request);

            // Diffuser la nouvelle position à tous les membres du trip
            var groupName = $"trip_{tripId}";
            await Clients.OthersInGroup(groupName).SendAsync("LocationUpdated", location);

            _logger.LogDebug("Location updated for user {UserId} in trip {TripId}", userId, tripId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating location");
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
