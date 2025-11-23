namespace SyncTrip.Mobile.Core.Services;

/// <summary>
/// Service de communication avec l'API backend via HttpClient.
/// Fournit des méthodes pour effectuer des requêtes HTTP typées.
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Effectue une requête POST et retourne une réponse typée.
    /// </summary>
    /// <typeparam name="TRequest">Type de la requête.</typeparam>
    /// <typeparam name="TResponse">Type de la réponse.</typeparam>
    /// <param name="endpoint">Endpoint de l'API (ex: "api/auth/verify").</param>
    /// <param name="request">Objet de requête à envoyer.</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>Réponse typée ou null en cas d'échec.</returns>
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken ct = default);

    /// <summary>
    /// Effectue une requête GET et retourne une réponse typée.
    /// </summary>
    /// <typeparam name="TResponse">Type de la réponse.</typeparam>
    /// <param name="endpoint">Endpoint de l'API.</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>Réponse typée ou null en cas d'échec.</returns>
    Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken ct = default);

    /// <summary>
    /// Effectue une requête POST sans attendre de réponse typée (succès/échec uniquement).
    /// </summary>
    /// <typeparam name="TRequest">Type de la requête.</typeparam>
    /// <param name="endpoint">Endpoint de l'API.</param>
    /// <param name="request">Objet de requête à envoyer.</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>True si la requête a réussi (2xx), False sinon.</returns>
    Task<bool> PostAsync<TRequest>(string endpoint, TRequest request, CancellationToken ct = default);
}
