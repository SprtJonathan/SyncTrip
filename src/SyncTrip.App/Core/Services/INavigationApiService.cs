using SyncTrip.Shared.DTOs.Navigation;

namespace SyncTrip.App.Core.Services;

public interface INavigationApiService
{
    Task<List<AddressResultDto>> SearchAddressAsync(string query, int limit = 5, CancellationToken ct = default);
    Task<RouteResultDto?> CalculateTripRouteAsync(Guid tripId, CancellationToken ct = default);
}
