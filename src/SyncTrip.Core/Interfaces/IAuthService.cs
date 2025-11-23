using SyncTrip.Core.Entities;

namespace SyncTrip.Core.Interfaces;

/// <summary>
/// Interface du service d'authentification.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Génère un token JWT pour un utilisateur.
    /// </summary>
    /// <param name="user">Utilisateur pour lequel générer le token.</param>
    /// <param name="additionalClaims">Claims additionnels à inclure (ex: scope registration).</param>
    /// <returns>Token JWT.</returns>
    string GenerateJwtToken(User user, Dictionary<string, string>? additionalClaims = null);
}
