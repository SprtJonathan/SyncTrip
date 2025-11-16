using SyncTrip.Api.Core.Entities;

namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Repository spécifique pour les participants de convoi
/// </summary>
public interface IConvoyParticipantRepository : IRepository<ConvoyParticipant>
{
    /// <summary>
    /// Récupère les participants actifs d'un convoi
    /// </summary>
    Task<IEnumerable<ConvoyParticipant>> GetConvoyParticipantsAsync(Guid convoyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère la participation d'un utilisateur dans un convoi
    /// </summary>
    Task<ConvoyParticipant?> GetUserParticipationAsync(Guid userId, Guid convoyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si un utilisateur est membre d'un convoi
    /// </summary>
    Task<bool> IsUserInConvoyAsync(Guid userId, Guid convoyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compte les participants actifs d'un convoi
    /// </summary>
    Task<int> CountActiveParticipantsAsync(Guid convoyId, CancellationToken cancellationToken = default);
}
