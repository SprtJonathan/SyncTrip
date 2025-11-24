using SyncTrip.Shared.DTOs.Users;

namespace SyncTrip.Mobile.Core.Services;

/// <summary>
/// Implémentation du service de gestion du profil utilisateur.
/// Communique avec l'API backend via IApiService.
/// </summary>
public class UserService : IUserService
{
    private readonly IApiService _apiService;

    /// <summary>
    /// Initialise une nouvelle instance du service utilisateur.
    /// </summary>
    /// <param name="apiService">Service API pour communiquer avec le backend.</param>
    public UserService(IApiService apiService)
    {
        _apiService = apiService;
    }

    /// <inheritdoc />
    public async Task<UserProfileDto?> GetProfileAsync(CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<UserProfileDto>("api/users/profile", ct);
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateProfileAsync(UpdateUserProfileRequest request, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.PostAsync("api/users/profile", request, ct);
        }
        catch
        {
            return false;
        }
    }
}
