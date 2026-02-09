namespace SyncTrip.Core.Enums;

/// <summary>
/// Statut d'un voyage (trip) dans un convoi.
/// </summary>
public enum TripStatus
{
    /// <summary>
    /// Voyage en cours avec enregistrement GPS actif.
    /// </summary>
    Recording = 1,

    /// <summary>
    /// Voyage en mode surveillance uniquement (pas d'enregistrement).
    /// </summary>
    MonitorOnly = 2,

    /// <summary>
    /// Voyage terminé.
    /// </summary>
    Finished = 3
}
