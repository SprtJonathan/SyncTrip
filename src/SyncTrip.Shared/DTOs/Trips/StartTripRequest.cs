namespace SyncTrip.Shared.DTOs.Trips;

/// <summary>
/// Requête pour démarrer un nouveau voyage.
/// </summary>
public record StartTripRequest
{
    /// <summary>
    /// Statut initial du voyage (1=Recording, 2=MonitorOnly).
    /// </summary>
    public int Status { get; init; }

    /// <summary>
    /// Profil de route (1=Fast, 2=Scenic).
    /// </summary>
    public int RouteProfile { get; init; }

    /// <summary>
    /// Waypoints initiaux (optionnels).
    /// </summary>
    public List<CreateWaypointRequest> Waypoints { get; init; } = new();
}
