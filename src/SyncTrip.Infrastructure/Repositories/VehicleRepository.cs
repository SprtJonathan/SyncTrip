using Microsoft.EntityFrameworkCore;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Interfaces;
using SyncTrip.Infrastructure.Persistence;

namespace SyncTrip.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les véhicules.
/// </summary>
public class VehicleRepository : IVehicleRepository
{
    private readonly ApplicationDbContext _context;

    public VehicleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Vehicles
            .Include(v => v.Brand)
            .FirstOrDefaultAsync(v => v.Id == id, ct);
    }

    public async Task<IList<Vehicle>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Vehicles
            .Include(v => v.Brand)
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken ct = default)
    {
        await _context.Vehicles.AddAsync(vehicle, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Vehicle vehicle, CancellationToken ct = default)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Vehicle vehicle, CancellationToken ct = default)
    {
        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync(ct);
    }
}
