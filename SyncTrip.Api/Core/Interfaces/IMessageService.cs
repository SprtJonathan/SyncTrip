using SyncTrip.Api.Application.DTOs.Messages;

namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Service de gestion des messages
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// Envoie un message dans un convoi
    /// </summary>
    Task<MessageDto> SendMessageAsync(Guid userId, Guid convoyId, SendMessageRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les messages d'un convoi (paginés)
    /// </summary>
    Task<IEnumerable<MessageDto>> GetConvoyMessagesAsync(Guid convoyId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les messages depuis une date
    /// </summary>
    Task<IEnumerable<MessageDto>> GetConvoyMessagesSinceAsync(Guid convoyId, DateTime since, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un message (soft delete)
    /// </summary>
    Task DeleteMessageAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default);
}
