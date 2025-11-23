using Microsoft.EntityFrameworkCore;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Interfaces;
using SyncTrip.Infrastructure.Persistence;

namespace SyncTrip.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les tokens Magic Link.
/// </summary>
public class MagicLinkTokenRepository : IMagicLinkTokenRepository
{
    private readonly ApplicationDbContext _context;

    public MagicLinkTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(MagicLinkToken token, CancellationToken cancellationToken = default)
    {
        await _context.MagicLinkTokens.AddAsync(token, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<MagicLinkToken?> GetByTokenAsync(string hashedToken, CancellationToken cancellationToken = default)
    {
        return await _context.MagicLinkTokens
            .FirstOrDefaultAsync(t => t.Token == hashedToken, cancellationToken);
    }

    public async Task MarkAsUsedAsync(MagicLinkToken token, CancellationToken cancellationToken = default)
    {
        _context.MagicLinkTokens.Update(token);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
