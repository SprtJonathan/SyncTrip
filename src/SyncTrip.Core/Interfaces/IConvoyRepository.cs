using SyncTrip.Core.Entities;

namespace SyncTrip.Core.Interfaces;

/// <summary>
/// Interface du repository pour les convois.
/// </summary>
public interface IConvoyRepository
{
    /// <summary>
    /// Récupère un convoi par son identifiant, avec ses membres.
    /// </summary>
    Task<Convoy?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Récupère un convoi par son code d'accès, avec ses membres.
    /// </summary>
    Task<Convoy?> GetByJoinCodeAsync(string joinCode, CancellationToken ct = default);

    /// <summary>
    /// Récupère tous les convois dont un utilisateur est membre.
    /// </summary>
    Task<IList<Convoy>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Vérifie si un code d'accès existe déjà.
    /// </summary>
    Task<bool> JoinCodeExistsAsync(string joinCode, CancellationToken ct = default);

    /// <summary>
    /// Ajoute un nouveau convoi.
    /// </summary>
    Task AddAsync(Convoy convoy, CancellationToken ct = default);

    /// <summary>
    /// Met à jour un convoi existant.
    /// </summary>
    Task UpdateAsync(Convoy convoy, CancellationToken ct = default);

    /// <summary>
    /// Supprime un convoi (dissolution).
    /// </summary>
    Task DeleteAsync(Convoy convoy, CancellationToken ct = default);
}
