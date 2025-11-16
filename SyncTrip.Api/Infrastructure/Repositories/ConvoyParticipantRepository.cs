using Microsoft.EntityFrameworkCore;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Interfaces;
using SyncTrip.Api.Infrastructure.Data;

namespace SyncTrip.Api.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les participants de convoi
/// </summary>
public class ConvoyParticipantRepository : Repository<ConvoyParticipant>, IConvoyParticipantRepository
{
    public ConvoyParticipantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ConvoyParticipant>> GetConvoyParticipantsAsync(Guid convoyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(cp => cp.User)
            .Where(cp => cp.ConvoyId == convoyId && cp.IsActive)
            .OrderBy(cp => cp.JoinedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ConvoyParticipant?> GetUserParticipationAsync(Guid userId, Guid convoyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(cp => cp.User)
            .Include(cp => cp.Convoy)
            .FirstOrDefaultAsync(cp => cp.UserId == userId && cp.ConvoyId == convoyId && cp.IsActive, cancellationToken);
    }

    public async Task<bool> IsUserInConvoyAsync(Guid userId, Guid convoyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(cp => cp.UserId == userId && cp.ConvoyId == convoyId && cp.IsActive, cancellationToken);
    }

    public async Task<int> CountActiveParticipantsAsync(Guid convoyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(cp => cp.ConvoyId == convoyId && cp.IsActive, cancellationToken);
    }
}
