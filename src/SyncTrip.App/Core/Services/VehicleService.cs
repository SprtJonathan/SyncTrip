using SyncTrip.Shared.DTOs.Vehicles;

namespace SyncTrip.App.Core.Services;

public class VehicleService : IVehicleService
{
    private readonly IApiService _apiService;

    public VehicleService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<VehicleDto>> GetVehiclesAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await _apiService.GetAsync<List<VehicleDto>>("api/vehicles", ct);
            return result ?? new List<VehicleDto>();
        }
        catch
        {
            return new List<VehicleDto>();
        }
    }

    public async Task<VehicleDto?> GetVehicleByIdAsync(Guid vehicleId, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<VehicleDto>($"api/vehicles/{vehicleId}", ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Guid?> CreateVehicleAsync(CreateVehicleRequest request, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.PostAsync<CreateVehicleRequest, Guid>("api/vehicles", request, ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateVehicleAsync(Guid vehicleId, UpdateVehicleRequest request, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.PutAsync($"api/vehicles/{vehicleId}", request, ct);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteVehicleAsync(Guid vehicleId, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.DeleteAsync($"api/vehicles/{vehicleId}", ct);
        }
        catch
        {
            return false;
        }
    }
}
