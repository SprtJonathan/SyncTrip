using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SyncTrip.API.Hubs;

/// <summary>
/// Hub SignalR pour la communication temps réel des voyages GPS.
/// Les positions sont éphémères (relayées via SignalR, non stockées en DB).
/// </summary>
[Authorize]
public class TripHub : Hub
{
    private readonly ILogger<TripHub> _logger;

    public TripHub(ILogger<TripHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Rejoint le groupe d'un voyage pour recevoir les mises à jour.
    /// </summary>
    /// <param name="tripId">Identifiant du voyage.</param>
    public async Task JoinTrip(Guid tripId)
    {
        var groupName = $"trip-{tripId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Connexion {ConnectionId} a rejoint le groupe {Group}", Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Quitte le groupe d'un voyage.
    /// </summary>
    /// <param name="tripId">Identifiant du voyage.</param>
    public async Task LeaveTrip(Guid tripId)
    {
        var groupName = $"trip-{tripId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Connexion {ConnectionId} a quitté le groupe {Group}", Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Envoie une mise à jour de position GPS aux autres membres du voyage.
    /// </summary>
    /// <param name="tripId">Identifiant du voyage.</param>
    /// <param name="latitude">Latitude actuelle.</param>
    /// <param name="longitude">Longitude actuelle.</param>
    public async Task SendLocationUpdate(Guid tripId, double latitude, double longitude)
    {
        var groupName = $"trip-{tripId}";
        await Clients.OthersInGroup(groupName).SendAsync("ReceiveLocationUpdate", new
        {
            UserId = Context.UserIdentifier,
            Latitude = latitude,
            Longitude = longitude,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Envoie une mise à jour de route (GeoJSON) aux autres membres du voyage.
    /// </summary>
    /// <param name="tripId">Identifiant du voyage.</param>
    /// <param name="geoJson">Données GeoJSON de la route.</param>
    public async Task SendRouteUpdate(Guid tripId, string geoJson)
    {
        var groupName = $"trip-{tripId}";
        await Clients.OthersInGroup(groupName).SendAsync("ReceiveRouteUpdate", new
        {
            UserId = Context.UserIdentifier,
            GeoJson = geoJson,
            Timestamp = DateTime.UtcNow
        });
    }
}
