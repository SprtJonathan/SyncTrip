using Microsoft.EntityFrameworkCore;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Enums;
using SyncTrip.Api.Core.Interfaces;
using SyncTrip.Api.Infrastructure.Data;

namespace SyncTrip.Api.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les convois
/// </summary>
public class ConvoyRepository : Repository<Convoy>, IConvoyRepository
{
    public ConvoyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Convoy?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Participants.Where(p => p.IsActive))
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Convoy>> GetUserConvoysAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Participants.Where(p => p.IsActive))
            .Where(c => c.Participants.Any(p => p.UserId == userId && p.IsActive))
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Convoy>> GetUserActiveConvoysAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Participants.Where(p => p.IsActive))
            .Where(c => c.Status == ConvoyStatus.Active &&
                       c.Participants.Any(p => p.UserId == userId && p.IsActive))
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters() // Vérifier même les convois supprimés
            .AnyAsync(c => c.Code == code, cancellationToken);
    }
}
