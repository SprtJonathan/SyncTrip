using Microsoft.EntityFrameworkCore;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Interfaces;
using SyncTrip.Infrastructure.Persistence;

namespace SyncTrip.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les convois.
/// </summary>
public class ConvoyRepository : IConvoyRepository
{
    private readonly ApplicationDbContext _context;

    public ConvoyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Convoy?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Convoys
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .Include(c => c.Members)
                .ThenInclude(m => m.Vehicle)
                    .ThenInclude(v => v.Brand)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Convoy?> GetByJoinCodeAsync(string joinCode, CancellationToken ct = default)
    {
        return await _context.Convoys
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .Include(c => c.Members)
                .ThenInclude(m => m.Vehicle)
                    .ThenInclude(v => v.Brand)
            .FirstOrDefaultAsync(c => c.JoinCode == joinCode, ct);
    }

    public async Task<IList<Convoy>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Convoys
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .Where(c => c.Members.Any(m => m.UserId == userId))
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<bool> JoinCodeExistsAsync(string joinCode, CancellationToken ct = default)
    {
        return await _context.Convoys.AnyAsync(c => c.JoinCode == joinCode, ct);
    }

    public async Task AddAsync(Convoy convoy, CancellationToken ct = default)
    {
        await _context.Convoys.AddAsync(convoy, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Convoy convoy, CancellationToken ct = default)
    {
        _context.Convoys.Update(convoy);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Convoy convoy, CancellationToken ct = default)
    {
        _context.Convoys.Remove(convoy);
        await _context.SaveChangesAsync(ct);
    }
}
