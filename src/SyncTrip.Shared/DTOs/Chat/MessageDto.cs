namespace SyncTrip.Shared.DTOs.Chat;

/// <summary>
/// DTO représentant un message de chat.
/// </summary>
public class MessageDto
{
    /// <summary>
    /// Identifiant unique du message.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifiant du convoi.
    /// </summary>
    public Guid ConvoyId { get; init; }

    /// <summary>
    /// Identifiant de l'expéditeur.
    /// </summary>
    public Guid SenderId { get; init; }

    /// <summary>
    /// Nom d'utilisateur de l'expéditeur.
    /// </summary>
    public string SenderUsername { get; init; } = string.Empty;

    /// <summary>
    /// URL de l'avatar de l'expéditeur.
    /// </summary>
    public string? SenderAvatarUrl { get; init; }

    /// <summary>
    /// Contenu du message.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Date et heure d'envoi.
    /// </summary>
    public DateTime SentAt { get; init; }
}
