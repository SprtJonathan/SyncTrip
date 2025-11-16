using Microsoft.EntityFrameworkCore;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Enums;
using SyncTrip.Api.Core.Interfaces;
using SyncTrip.Api.Infrastructure.Data;

namespace SyncTrip.Api.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les trips
/// </summary>
public class TripRepository : Repository<Trip>, ITripRepository
{
    public TripRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Trip>> GetConvoyTripsAsync(Guid convoyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ConvoyId == convoyId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Trip?> GetActiveConvoyTripAsync(Guid convoyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Waypoints.OrderBy(w => w.Order))
            .FirstOrDefaultAsync(t => t.ConvoyId == convoyId && t.Status == TripStatus.InProgress, cancellationToken);
    }

    public async Task<bool> HasActiveTripAsync(Guid convoyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(t => t.ConvoyId == convoyId && t.Status == TripStatus.InProgress, cancellationToken);
    }

    public async Task<Trip?> GetTripWithWaypointsAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Waypoints.OrderBy(w => w.Order))
            .Include(t => t.Convoy)
            .FirstOrDefaultAsync(t => t.Id == tripId, cancellationToken);
    }
}
