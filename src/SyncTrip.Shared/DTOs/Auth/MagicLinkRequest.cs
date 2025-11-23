namespace SyncTrip.Shared.DTOs.Auth;

/// <summary>
/// Requête pour envoyer un lien Magic Link.
/// </summary>
/// <param name="Email">Adresse email de l'utilisateur.</param>
public record MagicLinkRequest(string Email);
