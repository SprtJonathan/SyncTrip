using SyncTrip.Shared.DTOs.Auth;

namespace SyncTrip.Mobile.Core.Services;

/// <summary>
/// Implémentation du service d'authentification Magic Link.
/// Utilise l'ApiService pour communiquer avec le backend et SecureStorage pour stocker le JWT.
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IApiService _apiService;
    private const string TokenKey = "auth_token";

    /// <summary>
    /// Initialise une nouvelle instance du service d'authentification.
    /// </summary>
    /// <param name="apiService">Service API pour communiquer avec le backend.</param>
    public AuthenticationService(IApiService apiService)
    {
        _apiService = apiService;
    }

    /// <inheritdoc />
    public async Task<bool> SendMagicLinkAsync(string email)
    {
        try
        {
            var request = new MagicLinkRequest(email);
            return await _apiService.PostAsync("api/auth/magic-link", request);
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<VerifyTokenResponse?> VerifyTokenAsync(string token)
    {
        try
        {
            var request = new VerifyTokenRequest(token);
            return await _apiService.PostAsync<VerifyTokenRequest, VerifyTokenResponse>("api/auth/verify", request);
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<string?> CompleteRegistrationAsync(CompleteRegistrationRequest request)
    {
        try
        {
            var response = await _apiService.PostAsync<CompleteRegistrationRequest, VerifyTokenResponse>("api/auth/register", request);
            if (response?.Success == true && !string.IsNullOrEmpty(response.JwtToken))
            {
                await SaveTokenAsync(response.JwtToken);
                return response.JwtToken;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    /// <inheritdoc />
    public async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.GetAsync(TokenKey);
    }

    /// <inheritdoc />
    public async Task SaveTokenAsync(string token)
    {
        await SecureStorage.SetAsync(TokenKey, token);
    }

    /// <inheritdoc />
    public async Task ClearTokenAsync()
    {
        SecureStorage.Remove(TokenKey);
        await Task.CompletedTask;
    }
}
