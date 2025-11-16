using Microsoft.EntityFrameworkCore;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Interfaces;
using SyncTrip.Api.Infrastructure.Data;

namespace SyncTrip.Api.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les messages
/// </summary>
public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Message>> GetConvoyMessagesAsync(Guid convoyId, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.User)
            .Where(m => m.ConvoyId == convoyId)
            .OrderByDescending(m => m.SentAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetConvoyMessagesSinceAsync(Guid convoyId, DateTime since, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.User)
            .Where(m => m.ConvoyId == convoyId && m.SentAt > since)
            .OrderBy(m => m.SentAt)
            .ToListAsync(cancellationToken);
    }
}
