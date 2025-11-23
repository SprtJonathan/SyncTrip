using SyncTrip.Shared.DTOs.Auth;

namespace SyncTrip.Mobile.Core.Services;

/// <summary>
/// Service de gestion de l'authentification via Magic Link.
/// Gère l'envoi de liens magiques, la vérification de tokens, l'inscription et le stockage sécurisé des JWT.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Envoie un Magic Link à l'adresse email spécifiée.
    /// </summary>
    /// <param name="email">Adresse email de l'utilisateur.</param>
    /// <returns>True si l'envoi a réussi, False sinon.</returns>
    Task<bool> SendMagicLinkAsync(string email);

    /// <summary>
    /// Vérifie un token Magic Link.
    /// </summary>
    /// <param name="token">Token à vérifier.</param>
    /// <returns>Réponse de vérification contenant le JWT ou l'indicateur d'inscription requise.</returns>
    Task<VerifyTokenResponse?> VerifyTokenAsync(string token);

    /// <summary>
    /// Complète l'inscription d'un nouvel utilisateur.
    /// </summary>
    /// <param name="request">Données d'inscription (username, date de naissance, etc.).</param>
    /// <returns>Token JWT si l'inscription réussit, null sinon.</returns>
    Task<string?> CompleteRegistrationAsync(CompleteRegistrationRequest request);

    /// <summary>
    /// Vérifie si l'utilisateur est actuellement authentifié (possède un token valide).
    /// </summary>
    /// <returns>True si authentifié, False sinon.</returns>
    Task<bool> IsAuthenticatedAsync();

    /// <summary>
    /// Récupère le token JWT stocké de manière sécurisée.
    /// </summary>
    /// <returns>Token JWT ou null si non authentifié.</returns>
    Task<string?> GetTokenAsync();

    /// <summary>
    /// Sauvegarde le token JWT de manière sécurisée.
    /// </summary>
    /// <param name="token">Token JWT à sauvegarder.</param>
    Task SaveTokenAsync(string token);

    /// <summary>
    /// Supprime le token JWT stocké (déconnexion).
    /// </summary>
    Task ClearTokenAsync();
}
