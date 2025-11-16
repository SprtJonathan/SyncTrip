namespace SyncTrip.Api.Core.Enums;

/// <summary>
/// Statuts possibles d'un voyage
/// </summary>
public enum TripStatus
{
    /// <summary>
    /// Voyage planifié mais pas encore démarré
    /// </summary>
    Planned = 0,

    /// <summary>
    /// Voyage en cours
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Voyage en pause (arrêt temporaire)
    /// </summary>
    Paused = 2,

    /// <summary>
    /// Voyage terminé
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Voyage annulé
    /// </summary>
    Cancelled = 4
}
