using Microsoft.EntityFrameworkCore;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Interfaces;
using SyncTrip.Infrastructure.Persistence;

namespace SyncTrip.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les marques de véhicules.
/// </summary>
public class BrandRepository : IBrandRepository
{
    private readonly ApplicationDbContext _context;

    public BrandRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Brand?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Brands
            .FirstOrDefaultAsync(b => b.Id == id, ct);
    }

    public async Task<IList<Brand>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Brands
            .OrderBy(b => b.Name)
            .ToListAsync(ct);
    }
}
