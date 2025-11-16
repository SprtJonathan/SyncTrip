using SyncTrip.Api.Core.Entities;

namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Repository spécifique pour les tokens magic link
/// </summary>
public interface IMagicLinkTokenRepository : IRepository<MagicLinkToken>
{
    /// <summary>
    /// Récupère un token valide (non utilisé et non expiré)
    /// </summary>
    Task<MagicLinkToken?> GetValidTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime les tokens expirés
    /// </summary>
    Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default);
}
