using Microsoft.EntityFrameworkCore;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Interfaces;
using SyncTrip.Api.Infrastructure.Data;

namespace SyncTrip.Api.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour l'historique de localisation
/// </summary>
public class LocationHistoryRepository : Repository<LocationHistory>, ILocationHistoryRepository
{
    public LocationHistoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<LocationHistory?> GetLastUserLocationAsync(Guid userId, Guid tripId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(lh => lh.UserId == userId && lh.TripId == tripId)
            .OrderByDescending(lh => lh.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<LocationHistory>> GetTripParticipantsLastLocationsAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        // Récupère la dernière position de chaque participant du trip
        var lastLocations = await _dbSet
            .Include(lh => lh.User)
            .Where(lh => lh.TripId == tripId)
            .GroupBy(lh => lh.UserId)
            .Select(g => g.OrderByDescending(lh => lh.Timestamp).FirstOrDefault())
            .ToListAsync(cancellationToken);

        return lastLocations.Where(l => l != null).Cast<LocationHistory>();
    }

    public async Task DeleteOldHistoryAsync(int daysOld, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
        var oldHistory = await _dbSet
            .Where(lh => lh.Timestamp < cutoffDate)
            .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(oldHistory);
    }
}
