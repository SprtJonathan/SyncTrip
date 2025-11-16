using SyncTrip.Api.Core.Entities;

namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Repository spécifique pour les utilisateurs
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Récupère un utilisateur par son email
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si un email existe déjà
    /// </summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
