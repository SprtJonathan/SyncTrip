using SyncTrip.Shared.DTOs.Vehicles;

namespace SyncTrip.App.Core.Services;

public interface IVehicleService
{
    Task<List<VehicleDto>> GetVehiclesAsync(CancellationToken ct = default);
    Task<VehicleDto?> GetVehicleByIdAsync(Guid vehicleId, CancellationToken ct = default);
    Task<Guid?> CreateVehicleAsync(CreateVehicleRequest request, CancellationToken ct = default);
    Task<bool> UpdateVehicleAsync(Guid vehicleId, UpdateVehicleRequest request, CancellationToken ct = default);
    Task<bool> DeleteVehicleAsync(Guid vehicleId, CancellationToken ct = default);
}
