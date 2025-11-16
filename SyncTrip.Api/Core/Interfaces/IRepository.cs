using System.Linq.Expressions;
using SyncTrip.Api.Core.Entities;

namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Interface générique pour les repositories
/// Fournit les opérations CRUD de base
/// </summary>
/// <typeparam name="T">Type d'entité héritant de BaseEntity</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    // ===== LECTURE =====

    /// <summary>
    /// Récupère une entité par son ID
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère toutes les entités
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les entités selon un prédicat
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère une seule entité selon un prédicat
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si une entité existe selon un prédicat
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compte les entités selon un prédicat
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    // ===== ÉCRITURE =====

    /// <summary>
    /// Ajoute une nouvelle entité
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ajoute plusieurs entités
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour une entité
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Met à jour plusieurs entités
    /// </summary>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Supprime une entité
    /// </summary>
    void Remove(T entity);

    /// <summary>
    /// Supprime plusieurs entités
    /// </summary>
    void RemoveRange(IEnumerable<T> entities);
}
