using Microsoft.EntityFrameworkCore;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;
using SyncTrip.Infrastructure.Persistence;

namespace SyncTrip.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les propositions d'arrêt.
/// </summary>
public class StopProposalRepository : IStopProposalRepository
{
    private readonly ApplicationDbContext _context;

    public StopProposalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StopProposal?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.StopProposals
            .Include(p => p.Votes)
                .ThenInclude(v => v.User)
            .Include(p => p.ProposedByUser)
            .Include(p => p.Trip)
                .ThenInclude(t => t.Convoy)
                    .ThenInclude(c => c.Members)
            .Include(p => p.Trip)
                .ThenInclude(t => t.Waypoints)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<StopProposal?> GetPendingByTripIdAsync(Guid tripId, CancellationToken ct = default)
    {
        return await _context.StopProposals
            .Include(p => p.Votes)
                .ThenInclude(v => v.User)
            .Include(p => p.ProposedByUser)
            .FirstOrDefaultAsync(p => p.TripId == tripId && p.Status == ProposalStatus.Pending, ct);
    }

    public async Task<IList<StopProposal>> GetExpiredPendingAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await _context.StopProposals
            .Include(p => p.Votes)
                .ThenInclude(v => v.User)
            .Include(p => p.ProposedByUser)
            .Include(p => p.Trip)
                .ThenInclude(t => t.Convoy)
                    .ThenInclude(c => c.Members)
            .Include(p => p.Trip)
                .ThenInclude(t => t.Waypoints)
            .Where(p => p.Status == ProposalStatus.Pending && p.ExpiresAt <= now)
            .ToListAsync(ct);
    }

    public async Task<IList<StopProposal>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default)
    {
        return await _context.StopProposals
            .Include(p => p.Votes)
                .ThenInclude(v => v.User)
            .Include(p => p.ProposedByUser)
            .Where(p => p.TripId == tripId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(StopProposal proposal, CancellationToken ct = default)
    {
        await _context.StopProposals.AddAsync(proposal, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(StopProposal proposal, CancellationToken ct = default)
    {
        _context.StopProposals.Update(proposal);
        await _context.SaveChangesAsync(ct);
    }
}
