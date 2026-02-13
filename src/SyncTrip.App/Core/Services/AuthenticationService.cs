using SyncTrip.App.Core.Platform;
using SyncTrip.Shared.DTOs.Auth;

namespace SyncTrip.App.Core.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IApiService _apiService;
    private readonly ISecureStorageService _secureStorage;
    private const string TokenKey = "auth_token";

    public AuthenticationService(IApiService apiService, ISecureStorageService secureStorage)
    {
        _apiService = apiService;
        _secureStorage = secureStorage;
    }

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

    public async Task<string?> CompleteRegistrationAsync(CompleteRegistrationRequest request)
    {
        try
        {
            var response = await _apiService.PostAsync<CompleteRegistrationRequest, VerifyTokenResponse>("api/auth/register", request);
            if (response is not null && !string.IsNullOrEmpty(response.JwtToken))
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

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _secureStorage.GetAsync(TokenKey);
    }

    public async Task SaveTokenAsync(string token)
    {
        await _secureStorage.SetAsync(TokenKey, token);
    }

    public Task ClearTokenAsync()
    {
        _secureStorage.Remove(TokenKey);
        return Task.CompletedTask;
    }
}
