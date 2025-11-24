using SyncTrip.Core.Entities;

namespace SyncTrip.Core.Interfaces;

/// <summary>
/// Interface du repository pour la gestion des utilisateurs.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Récupère un utilisateur par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant de l'utilisateur.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Utilisateur ou null si non trouvé.</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un utilisateur par son email.
    /// </summary>
    /// <param name="email">Adresse email de l'utilisateur.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Utilisateur ou null si non trouvé.</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ajoute un nouvel utilisateur.
    /// </summary>
    /// <param name="user">Utilisateur à ajouter.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un utilisateur existant.
    /// </summary>
    /// <param name="user">Utilisateur à mettre à jour.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un utilisateur par son identifiant avec ses permis de conduire.
    /// </summary>
    /// <param name="id">Identifiant de l'utilisateur.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Utilisateur avec ses permis ou null si non trouvé.</returns>
    Task<User?> GetByIdWithLicensesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour les permis de conduire d'un utilisateur.
    /// Remplace la liste existante par la nouvelle liste fournie.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="licenseTypes">Liste des types de permis.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task UpdateUserLicensesAsync(Guid userId, IList<Core.Enums.LicenseType> licenseTypes, CancellationToken cancellationToken = default);
}
