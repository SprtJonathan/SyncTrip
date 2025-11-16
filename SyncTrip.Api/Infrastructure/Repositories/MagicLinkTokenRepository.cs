using Microsoft.EntityFrameworkCore;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Interfaces;
using SyncTrip.Api.Infrastructure.Data;

namespace SyncTrip.Api.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les tokens magic link
/// </summary>
public class MagicLinkTokenRepository : Repository<MagicLinkToken>, IMagicLinkTokenRepository
{
    public MagicLinkTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<MagicLinkToken?> GetValidTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.User)
            .FirstOrDefaultAsync(t =>
                t.Token == token &&
                !t.IsUsed &&
                t.ExpiresAt > DateTime.UtcNow,
                cancellationToken);
    }

    public async Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var expiredTokens = await _dbSet
            .Where(t => t.ExpiresAt < DateTime.UtcNow || t.IsUsed)
            .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(expiredTokens);
    }
}
