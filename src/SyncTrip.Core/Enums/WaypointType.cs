namespace SyncTrip.Core.Enums;

/// <summary>
/// Type de point de passage dans un itinéraire.
/// </summary>
public enum WaypointType
{
    /// <summary>
    /// Point de départ.
    /// </summary>
    Start = 1,

    /// <summary>
    /// Étape intermédiaire.
    /// </summary>
    Stopover = 2,

    /// <summary>
    /// Destination finale.
    /// </summary>
    Destination = 3
}
