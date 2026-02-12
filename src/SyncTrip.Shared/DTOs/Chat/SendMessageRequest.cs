namespace SyncTrip.Shared.DTOs.Chat;

/// <summary>
/// Requête pour envoyer un message dans le chat d'un convoi.
/// </summary>
public record SendMessageRequest
{
    /// <summary>
    /// Contenu du message (max 500 caractères).
    /// </summary>
    public string Content { get; init; } = string.Empty;
}
