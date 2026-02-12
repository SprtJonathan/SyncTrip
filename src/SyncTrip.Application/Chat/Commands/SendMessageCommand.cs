using MediatR;

namespace SyncTrip.Application.Chat.Commands;

/// <summary>
/// Command pour envoyer un message dans le chat d'un convoi.
/// </summary>
public record SendMessageCommand : IRequest<Guid>
{
    /// <summary>
    /// Identifiant du convoi.
    /// </summary>
    public Guid ConvoyId { get; init; }

    /// <summary>
    /// Identifiant de l'utilisateur expéditeur.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Contenu du message.
    /// </summary>
    public string Content { get; init; } = string.Empty;
}
