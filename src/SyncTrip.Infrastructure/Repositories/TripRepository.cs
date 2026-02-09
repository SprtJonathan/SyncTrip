using Microsoft.EntityFrameworkCore;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;
using SyncTrip.Infrastructure.Persistence;

namespace SyncTrip.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les voyages (trips).
/// </summary>
public class TripRepository : ITripRepository
{
    private readonly ApplicationDbContext _context;

    public TripRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Trip?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Trips
            .Include(t => t.Convoy)
                .ThenInclude(c => c.Members)
            .Include(t => t.Waypoints)
                .ThenInclude(w => w.AddedByUser)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<Trip?> GetActiveByConvoyIdAsync(Guid convoyId, CancellationToken ct = default)
    {
        return await _context.Trips
            .Include(t => t.Convoy)
                .ThenInclude(c => c.Members)
            .Include(t => t.Waypoints)
                .ThenInclude(w => w.AddedByUser)
            .FirstOrDefaultAsync(t => t.ConvoyId == convoyId && t.Status != TripStatus.Finished, ct);
    }

    public async Task<IList<Trip>> GetByConvoyIdAsync(Guid convoyId, CancellationToken ct = default)
    {
        return await _context.Trips
            .Include(t => t.Waypoints)
            .Where(t => t.ConvoyId == convoyId)
            .OrderByDescending(t => t.StartTime)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Trip trip, CancellationToken ct = default)
    {
        await _context.Trips.AddAsync(trip, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Trip trip, CancellationToken ct = default)
    {
        _context.Trips.Update(trip);
        await _context.SaveChangesAsync(ct);
    }
}
