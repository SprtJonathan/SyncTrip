using SyncTrip.Shared.DTOs.Users;

namespace SyncTrip.App.Core.Services;

public interface IUserService
{
    Task<UserProfileDto?> GetProfileAsync(CancellationToken ct = default);
    Task<bool> UpdateProfileAsync(UpdateUserProfileRequest request, CancellationToken ct = default);
}
