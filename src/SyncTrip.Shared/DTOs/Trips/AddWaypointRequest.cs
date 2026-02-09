namespace SyncTrip.Shared.DTOs.Trips;

/// <summary>
/// Requête pour ajouter un waypoint à un voyage existant.
/// </summary>
public record AddWaypointRequest
{
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
}
