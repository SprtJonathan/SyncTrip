namespace SyncTrip.Core.Enums;

/// <summary>
/// Type d'arrêt proposé lors d'un vote.
/// </summary>
public enum StopType
{
    /// <summary>
    /// Arrêt pour faire le plein d'essence.
    /// </summary>
    Fuel = 1,

    /// <summary>
    /// Pause générale.
    /// </summary>
    Break = 2,

    /// <summary>
    /// Arrêt repas.
    /// </summary>
    Food = 3,

    /// <summary>
    /// Arrêt photo.
    /// </summary>
    Photo = 4
}
