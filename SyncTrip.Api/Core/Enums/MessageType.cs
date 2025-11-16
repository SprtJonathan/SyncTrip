namespace SyncTrip.Api.Core.Enums;

/// <summary>
/// Types de messages dans le chat
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Message système automatique (ex: "X a rejoint le convoi")
    /// </summary>
    System = 0,

    /// <summary>
    /// Message envoyé par un utilisateur
    /// </summary>
    User = 1
}
