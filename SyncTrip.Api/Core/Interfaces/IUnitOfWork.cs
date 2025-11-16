namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Interface pour le pattern Unit of Work
/// Gère les transactions et coordonne les repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Repository pour les utilisateurs
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Repository pour les convois
    /// </summary>
    IConvoyRepository Convoys { get; }

    /// <summary>
    /// Repository pour les trips
    /// </summary>
    ITripRepository Trips { get; }

    /// <summary>
    /// Repository pour les participants de convoi
    /// </summary>
    IConvoyParticipantRepository ConvoyParticipants { get; }

    /// <summary>
    /// Repository pour les waypoints
    /// </summary>
    IWaypointRepository Waypoints { get; }

    /// <summary>
    /// Repository pour les tokens magic link
    /// </summary>
    IMagicLinkTokenRepository MagicLinkTokens { get; }

    /// <summary>
    /// Repository pour l'historique de localisation
    /// </summary>
    ILocationHistoryRepository LocationHistories { get; }

    /// <summary>
    /// Repository pour les messages
    /// </summary>
    IMessageRepository Messages { get; }

    /// <summary>
    /// Sauvegarde tous les changements en base de données
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Démarre une transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit la transaction en cours
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback la transaction en cours
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
