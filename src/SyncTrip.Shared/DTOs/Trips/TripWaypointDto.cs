namespace SyncTrip.Shared.DTOs.Trips;

/// <summary>
/// DTO représentant un point de passage.
/// </summary>
public class TripWaypointDto
{
    /// <summary>
    /// Identifiant unique du waypoint.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Index d'ordre dans l'itinéraire.
    /// </summary>
    public int OrderIndex { get; init; }

    /// <summary>
    /// Latitude du point de passage.
    /// </summary>
    public double Latitude { get; init; }

    /// <summary>
    /// Longitude du point de passage.
    /// </summary>
    public double Longitude { get; init; }

    /// <summary>
    /// Nom du point de passage.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Type de waypoint (1=Start, 2=Stopover, 3=Destination).
    /// </summary>
    public int Type { get; init; }

    /// <summary>
    /// Identifiant de l'utilisateur ayant ajouté ce waypoint.
    /// </summary>
    public Guid AddedByUserId { get; init; }

    /// <summary>
    /// Nom d'utilisateur ayant ajouté ce waypoint.
    /// </summary>
    public string AddedByUsername { get; init; } = string.Empty;
}
