namespace SyncTrip.Core.Enums;

/// <summary>
/// Statut d'une proposition d'arrêt.
/// </summary>
public enum ProposalStatus
{
    /// <summary>
    /// Vote en cours.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Proposition acceptée.
    /// </summary>
    Accepted = 2,

    /// <summary>
    /// Proposition rejetée.
    /// </summary>
    Rejected = 3
}
