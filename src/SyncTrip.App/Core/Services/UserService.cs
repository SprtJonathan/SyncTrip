using SyncTrip.Shared.DTOs.Users;

namespace SyncTrip.App.Core.Services;

public class UserService : IUserService
{
    private readonly IApiService _apiService;

    public UserService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<UserProfileDto?> GetProfileAsync(CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<UserProfileDto>("api/users/me", ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateProfileAsync(UpdateUserProfileRequest request, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.PutAsync("api/users/me", request, ct);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAccountAsync(CancellationToken ct = default)
    {
        try
        {
            return await _apiService.DeleteAsync("api/users/me", ct);
        }
        catch
        {
            return false;
        }
    }
}
