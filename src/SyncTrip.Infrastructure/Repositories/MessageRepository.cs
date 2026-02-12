using Microsoft.EntityFrameworkCore;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Interfaces;
using SyncTrip.Infrastructure.Persistence;

namespace SyncTrip.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les messages de chat.
/// </summary>
public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _context;

    public MessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Message>> GetByConvoyIdAsync(Guid convoyId, int pageSize, DateTime? before = null, CancellationToken ct = default)
    {
        var query = _context.Messages
            .Include(m => m.Sender)
            .Where(m => m.ConvoyId == convoyId);

        if (before.HasValue)
            query = query.Where(m => m.SentAt < before.Value);

        return await query
            .OrderByDescending(m => m.SentAt)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Message message, CancellationToken ct = default)
    {
        await _context.Messages.AddAsync(message, ct);
        await _context.SaveChangesAsync(ct);
    }
}
