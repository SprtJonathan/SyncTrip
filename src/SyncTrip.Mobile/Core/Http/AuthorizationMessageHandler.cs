using System.Net.Http.Headers;

namespace SyncTrip.Mobile.Core.Http;

/// <summary>
/// Handler HTTP qui ajoute automatiquement le token JWT
/// dans l'en-tête Authorization de chaque requête sortante.
/// Lit le token directement depuis SecureStorage pour éviter
/// une dépendance circulaire avec IAuthenticationService.
/// </summary>
public class AuthorizationMessageHandler : DelegatingHandler
{
    private const string TokenKey = "auth_token";

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await SecureStorage.GetAsync(TokenKey);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
