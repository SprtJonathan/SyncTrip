using SyncTrip.Api.Application.DTOs.Auth;

namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Service d'authentification passwordless
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Enregistre un nouvel utilisateur
    /// </summary>
    Task<UserDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envoie un magic link par email
    /// </summary>
    Task SendMagicLinkAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie le token magic link et retourne les tokens JWT
    /// </summary>
    Task<AuthResponse> VerifyMagicLinkAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rafraîchit le token JWT
    /// </summary>
    Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Génère un token JWT pour un utilisateur
    /// </summary>
    string GenerateJwtToken(Guid userId, string email);

    /// <summary>
    /// Génère un refresh token
    /// </summary>
    string GenerateRefreshToken();
}
