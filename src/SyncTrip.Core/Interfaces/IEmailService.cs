namespace SyncTrip.Core.Interfaces;

/// <summary>
/// Interface du service d'envoi d'emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envoie un email avec un lien Magic Link.
    /// </summary>
    /// <param name="email">Adresse email du destinataire.</param>
    /// <param name="token">Token Magic Link à inclure dans l'URL.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task SendMagicLinkAsync(string email, string token, CancellationToken cancellationToken = default);
}
