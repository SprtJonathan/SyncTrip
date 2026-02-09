namespace SyncTrip.Shared.DTOs.Trips;

/// <summary>
/// DTO représentant un voyage avec ses détails complets.
/// </summary>
public class TripDetailsDto
{
    /// <summary>
    /// Identifiant unique du voyage.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifiant du convoi.
    /// </summary>
    public Guid ConvoyId { get; init; }

    /// <summary>
    /// Statut du voyage (1=Recording, 2=MonitorOnly, 3=Finished).
    /// </summary>
    public int Status { get; init; }

    /// <summary>
    /// Date/heure de début.
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// Date/heure de fin (null si en cours).
    /// </summary>
    public DateTime? EndTime { get; init; }

    /// <summary>
    /// Profil de route utilisé (1=Fast, 2=Scenic).
    /// </summary>
    public int RouteProfile { get; init; }

    /// <summary>
    /// Nombre de waypoints.
    /// </summary>
    public int WaypointCount { get; init; }

    /// <summary>
    /// Liste des waypoints ordonnés.
    /// </summary>
    public List<TripWaypointDto> Waypoints { get; init; } = new();
}
