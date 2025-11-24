using SyncTrip.Shared.DTOs.Brands;

namespace SyncTrip.Mobile.Core.Services;

/// <summary>
/// Implémentation du service de gestion des marques.
/// Communique avec l'API backend via IApiService.
/// </summary>
public class BrandService : IBrandService
{
    private readonly IApiService _apiService;

    /// <summary>
    /// Initialise une nouvelle instance du service marques.
    /// </summary>
    /// <param name="apiService">Service API pour communiquer avec le backend.</param>
    public BrandService(IApiService apiService)
    {
        _apiService = apiService;
    }

    /// <inheritdoc />
    public async Task<List<BrandDto>> GetBrandsAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await _apiService.GetAsync<List<BrandDto>>("api/brands", ct);
            return result ?? new List<BrandDto>();
        }
        catch
        {
            return new List<BrandDto>();
        }
    }

    /// <inheritdoc />
    public async Task<BrandDto?> GetBrandByIdAsync(int brandId, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<BrandDto>($"api/brands/{brandId}", ct);
        }
        catch
        {
            return null;
        }
    }
}
