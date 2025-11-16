namespace SyncTrip.Api.Core.Enums;

/// <summary>
/// Rôles possibles d'un participant dans un convoi
/// </summary>
public enum ConvoyRole
{
    /// <summary>
    /// Créateur du convoi - tous les droits
    /// </summary>
    Owner = 0,

    /// <summary>
    /// Administrateur - peut gérer les participants et les trips
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Membre standard - peut voir et participer
    /// </summary>
    Member = 2
}
