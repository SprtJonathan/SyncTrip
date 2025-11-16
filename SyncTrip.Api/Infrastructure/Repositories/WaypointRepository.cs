using Microsoft.EntityFrameworkCore;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Interfaces;
using SyncTrip.Api.Infrastructure.Data;

namespace SyncTrip.Api.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les waypoints
/// </summary>
public class WaypointRepository : Repository<Waypoint>, IWaypointRepository
{
    public WaypointRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Waypoint>> GetTripWaypointsAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => w.TripId == tripId)
            .OrderBy(w => w.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Waypoint>> GetUnreachedWaypointsAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => w.TripId == tripId && !w.IsReached)
            .OrderBy(w => w.Order)
            .ToListAsync(cancellationToken);
    }
}
