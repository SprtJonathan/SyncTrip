using SyncTrip.Api.Core.Entities;

namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Repository spécifique pour les messages
/// </summary>
public interface IMessageRepository : IRepository<Message>
{
    /// <summary>
    /// Récupère les messages d'un convoi, paginés
    /// </summary>
    Task<IEnumerable<Message>> GetConvoyMessagesAsync(Guid convoyId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les messages d'un convoi depuis une date
    /// </summary>
    Task<IEnumerable<Message>> GetConvoyMessagesSinceAsync(Guid convoyId, DateTime since, CancellationToken cancellationToken = default);
}
