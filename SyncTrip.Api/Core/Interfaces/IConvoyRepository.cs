using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Repository spécifique pour les convois
/// </summary>
public interface IConvoyRepository : IRepository<Convoy>
{
    /// <summary>
    /// Récupère un convoi par son code unique
    /// </summary>
    Task<Convoy?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les convois d'un utilisateur
    /// </summary>
    Task<IEnumerable<Convoy>> GetUserConvoysAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les convois actifs d'un utilisateur
    /// </summary>
    Task<IEnumerable<Convoy>> GetUserActiveConvoysAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si un code de convoi existe déjà
    /// </summary>
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
}
