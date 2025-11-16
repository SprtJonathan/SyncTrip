namespace SyncTrip.Api.Core.Enums;

/// <summary>
/// Statuts possibles d'un convoi
/// </summary>
public enum ConvoyStatus
{
    /// <summary>
    /// Convoi actif avec au moins un participant
    /// </summary>
    Active = 0,

    /// <summary>
    /// Convoi archivé (aucun participant depuis 30 jours)
    /// </summary>
    Archived = 1,

    /// <summary>
    /// Convoi supprimé (soft delete, archivé depuis 1 an)
    /// </summary>
    Deleted = 2
}
