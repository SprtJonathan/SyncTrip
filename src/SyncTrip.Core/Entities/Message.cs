using SyncTrip.Core.Exceptions;

namespace SyncTrip.Core.Entities;

/// <summary>
/// Représente un message de chat dans un convoi.
/// </summary>
public class Message
{
    /// <summary>
    /// Identifiant unique du message.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identifiant du convoi auquel appartient le message.
    /// </summary>
    public Guid ConvoyId { get; private set; }

    /// <summary>
    /// Navigation vers le convoi.
    /// </summary>
    public Convoy Convoy { get; private set; } = null!;

    /// <summary>
    /// Identifiant de l'expéditeur.
    /// </summary>
    public Guid SenderId { get; private set; }

    /// <summary>
    /// Navigation vers l'utilisateur expéditeur.
    /// </summary>
    public User Sender { get; private set; } = null!;

    /// <summary>
    /// Contenu du message (max 500 caractères).
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Date et heure d'envoi du message.
    /// </summary>
    public DateTime SentAt { get; private set; }

    /// <summary>
    /// Longueur maximale du contenu d'un message.
    /// </summary>
    private const int MaxContentLength = 500;

    // Constructeur privé pour EF Core
    private Message()
    {
    }

    /// <summary>
    /// Factory method pour créer un nouveau message.
    /// </summary>
    /// <param name="convoyId">Identifiant du convoi.</param>
    /// <param name="senderId">Identifiant de l'expéditeur.</param>
    /// <param name="content">Contenu du message.</param>
    /// <returns>Nouvelle instance de Message.</returns>
    public static Message Create(Guid convoyId, Guid senderId, string content)
    {
        if (convoyId == Guid.Empty)
            throw new ArgumentException("L'identifiant du convoi est invalide.", nameof(convoyId));

        if (senderId == Guid.Empty)
            throw new ArgumentException("L'identifiant de l'expéditeur est invalide.", nameof(senderId));

        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Le message ne peut pas etre vide.");

        if (content.Length > MaxContentLength)
            throw new DomainException("Le message ne peut pas depasser 500 caracteres.");

        return new Message
        {
            Id = Guid.NewGuid(),
            ConvoyId = convoyId,
            SenderId = senderId,
            Content = content,
            SentAt = DateTime.UtcNow
        };
    }
}
