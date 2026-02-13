using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.App.Core.Services;

public class TripService : ITripService
{
    private readonly IApiService _apiService;

    public TripService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<Guid?> StartTripAsync(Guid convoyId, StartTripRequest request, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.PostAsync<StartTripRequest, Guid>($"api/convoys/{convoyId}/trips", request, ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<TripDetailsDto?> GetActiveTripAsync(Guid convoyId, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<TripDetailsDto>($"api/convoys/{convoyId}/trips/active", ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<TripDetailsDto?> GetTripByIdAsync(Guid convoyId, Guid tripId, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<TripDetailsDto>($"api/convoys/{convoyId}/trips/{tripId}", ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<TripDto>> GetConvoyTripsAsync(Guid convoyId, CancellationToken ct = default)
    {
        try
        {
            var result = await _apiService.GetAsync<List<TripDto>>($"api/convoys/{convoyId}/trips", ct);
            return result ?? new List<TripDto>();
        }
        catch
        {
            return new List<TripDto>();
        }
    }

    public async Task<bool> EndTripAsync(Guid convoyId, Guid tripId, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.PostAsync($"api/convoys/{convoyId}/trips/{tripId}/end", new { }, ct);
        }
        catch
        {
            return false;
        }
    }
}
