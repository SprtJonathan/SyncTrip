using SyncTrip.Core.Entities;

namespace SyncTrip.Core.Interfaces;

/// <summary>
/// Interface du repository pour la gestion des tokens Magic Link.
/// </summary>
public interface IMagicLinkTokenRepository
{
    /// <summary>
    /// Crée un nouveau token Magic Link.
    /// </summary>
    /// <param name="token">Token à créer.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task CreateAsync(MagicLinkToken token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un token par sa valeur hashée.
    /// </summary>
    /// <param name="hashedToken">Token hashé.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Token ou null si non trouvé.</returns>
    Task<MagicLinkToken?> GetByTokenAsync(string hashedToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marque un token comme utilisé.
    /// </summary>
    /// <param name="token">Token à marquer comme utilisé.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task MarkAsUsedAsync(MagicLinkToken token, CancellationToken cancellationToken = default);
}
