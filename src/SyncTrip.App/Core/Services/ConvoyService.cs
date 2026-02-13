using SyncTrip.Shared.DTOs.Convoys;

namespace SyncTrip.App.Core.Services;

public class ConvoyService : IConvoyService
{
    private readonly IApiService _apiService;

    public ConvoyService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<Guid?> CreateConvoyAsync(CreateConvoyRequest request, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.PostAsync<CreateConvoyRequest, Guid>("api/convoys", request, ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ConvoyDetailsDto?> GetConvoyByCodeAsync(string joinCode, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<ConvoyDetailsDto>($"api/convoys/{joinCode}", ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<ConvoyDto>> GetMyConvoysAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await _apiService.GetAsync<List<ConvoyDto>>("api/convoys/my", ct);
            return result ?? new List<ConvoyDto>();
        }
        catch
        {
            return new List<ConvoyDto>();
        }
    }

    public async Task<bool> JoinConvoyAsync(string joinCode, JoinConvoyRequest request, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.PostAsync($"api/convoys/{joinCode}/join", request, ct);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> LeaveConvoyAsync(string joinCode, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.PostAsync($"api/convoys/{joinCode}/leave", new { }, ct);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DissolveConvoyAsync(string joinCode, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.DeleteAsync($"api/convoys/{joinCode}", ct);
        }
        catch
        {
            return false;
        }
    }
}
