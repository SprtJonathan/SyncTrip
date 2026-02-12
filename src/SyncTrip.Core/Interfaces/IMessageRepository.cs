using SyncTrip.Core.Entities;

namespace SyncTrip.Core.Interfaces;

/// <summary>
/// Interface du repository pour les messages de chat.
/// </summary>
public interface IMessageRepository
{
    /// <summary>
    /// Récupère les messages d'un convoi avec pagination par curseur.
    /// </summary>
    /// <param name="convoyId">Identifiant du convoi.</param>
    /// <param name="pageSize">Nombre de messages à récupérer.</param>
    /// <param name="before">Date avant laquelle récupérer les messages (curseur).</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>Liste des messages triés par date décroissante.</returns>
    Task<IList<Message>> GetByConvoyIdAsync(Guid convoyId, int pageSize, DateTime? before = null, CancellationToken ct = default);

    /// <summary>
    /// Ajoute un nouveau message.
    /// </summary>
    Task AddAsync(Message message, CancellationToken ct = default);
}
