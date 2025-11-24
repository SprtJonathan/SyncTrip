using SyncTrip.Shared.DTOs.Vehicles;

namespace SyncTrip.Mobile.Core.Services;

/// <summary>
/// Implémentation du service de gestion des véhicules.
/// Communique avec l'API backend via IApiService.
/// </summary>
public class VehicleService : IVehicleService
{
    private readonly IApiService _apiService;

    /// <summary>
    /// Initialise une nouvelle instance du service véhicule.
    /// </summary>
    /// <param name="apiService">Service API pour communiquer avec le backend.</param>
    public VehicleService(IApiService apiService)
    {
        _apiService = apiService;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<Guid?> CreateVehicleAsync(CreateVehicleRequest request, CancellationToken ct = default)
    {
        try
        {
            var response = await _apiService.PostAsync<CreateVehicleRequest, Guid>("api/vehicles", request, ct);
            return response;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateVehicleAsync(Guid vehicleId, UpdateVehicleRequest request, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.PostAsync($"api/vehicles/{vehicleId}", request, ct);
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteVehicleAsync(Guid vehicleId, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.PostAsync($"api/vehicles/{vehicleId}/delete", new { }, ct);
        }
        catch
        {
            return false;
        }
    }
}
