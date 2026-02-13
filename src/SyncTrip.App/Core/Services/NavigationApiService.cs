using SyncTrip.Shared.DTOs.Navigation;

namespace SyncTrip.App.Core.Services;

public class NavigationApiService : INavigationApiService
{
    private readonly IApiService _apiService;

    public NavigationApiService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<AddressResultDto>> SearchAddressAsync(string query, int limit = 5, CancellationToken ct = default)
    {
        try
        {
            var encoded = Uri.EscapeDataString(query);
            var result = await _apiService.GetAsync<List<AddressResultDto>>($"api/navigation/search?query={encoded}&limit={limit}", ct);
            return result ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<RouteResultDto?> CalculateTripRouteAsync(Guid tripId, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.PostAsync<object, RouteResultDto>($"api/navigation/trips/{tripId}/route", new { }, ct);
        }
        catch
        {
            return null;
        }
    }
}
