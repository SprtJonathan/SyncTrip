using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.App.Core.Services;

public interface ITripService
{
    Task<Guid?> StartTripAsync(Guid convoyId, StartTripRequest request, CancellationToken ct = default);
    Task<TripDetailsDto?> GetActiveTripAsync(Guid convoyId, CancellationToken ct = default);
    Task<TripDetailsDto?> GetTripByIdAsync(Guid convoyId, Guid tripId, CancellationToken ct = default);
    Task<List<TripDto>> GetConvoyTripsAsync(Guid convoyId, CancellationToken ct = default);
    Task<bool> EndTripAsync(Guid convoyId, Guid tripId, CancellationToken ct = default);
}
