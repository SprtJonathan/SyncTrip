using Microsoft.EntityFrameworkCore.Storage;
using SyncTrip.Api.Core.Interfaces;
using SyncTrip.Api.Infrastructure.Repositories;

namespace SyncTrip.Api.Infrastructure.Data;

/// <summary>
/// Implémentation du pattern Unit of Work
/// Coordonne les repositories et gère les transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repositories (lazy initialization)
    private IUserRepository? _users;
    private IConvoyRepository? _convoys;
    private ITripRepository? _trips;
    private IConvoyParticipantRepository? _convoyParticipants;
    private IWaypointRepository? _waypoints;
    private IMagicLinkTokenRepository? _magicLinkTokens;
    private IRefreshTokenRepository? _refreshTokens;
    private ILocationHistoryRepository? _locationHistories;
    private IMessageRepository? _messages;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // Propriétés des repositories avec lazy initialization
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IConvoyRepository Convoys => _convoys ??= new ConvoyRepository(_context);
    public ITripRepository Trips => _trips ??= new TripRepository(_context);
    public IConvoyParticipantRepository ConvoyParticipants => _convoyParticipants ??= new ConvoyParticipantRepository(_context);
    public IWaypointRepository Waypoints => _waypoints ??= new WaypointRepository(_context);
    public IMagicLinkTokenRepository MagicLinkTokens => _magicLinkTokens ??= new MagicLinkTokenRepository(_context);
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);
    public ILocationHistoryRepository LocationHistories => _locationHistories ??= new LocationHistoryRepository(_context);
    public IMessageRepository Messages => _messages ??= new MessageRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
