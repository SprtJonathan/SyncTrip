using SyncTrip.Shared.DTOs.Brands;

namespace SyncTrip.App.Core.Services;

public interface IBrandService
{
    Task<List<BrandDto>> GetBrandsAsync(CancellationToken ct = default);
    Task<BrandDto?> GetBrandByIdAsync(int brandId, CancellationToken ct = default);
}
