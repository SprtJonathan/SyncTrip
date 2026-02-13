using SyncTrip.Shared.DTOs.Brands;

namespace SyncTrip.App.Core.Services;

public class BrandService : IBrandService
{
    private readonly IApiService _apiService;

    public BrandService(IApiService apiService)
    {
        _apiService = apiService;
    }

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
