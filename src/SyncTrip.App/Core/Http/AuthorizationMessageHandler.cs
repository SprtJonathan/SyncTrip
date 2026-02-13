using System.Net.Http.Headers;
using SyncTrip.App.Core.Platform;

namespace SyncTrip.App.Core.Http;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly ISecureStorageService _secureStorage;
    private const string TokenKey = "auth_token";

    public AuthorizationMessageHandler(ISecureStorageService secureStorage)
    {
        _secureStorage = secureStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _secureStorage.GetAsync(TokenKey);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
