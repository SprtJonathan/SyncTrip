using SyncTrip.Shared.DTOs.Auth;

namespace SyncTrip.App.Core.Services;

public interface IAuthenticationService
{
    Task<bool> SendMagicLinkAsync(string email);
    Task<VerifyTokenResponse?> VerifyTokenAsync(string token);
    Task<string?> CompleteRegistrationAsync(CompleteRegistrationRequest request);
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetTokenAsync();
    Task SaveTokenAsync(string token);
    Task ClearTokenAsync();
}
