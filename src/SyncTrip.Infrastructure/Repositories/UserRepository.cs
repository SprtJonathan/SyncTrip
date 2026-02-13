using Microsoft.EntityFrameworkCore;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Interfaces;
using SyncTrip.Infrastructure.Persistence;

namespace SyncTrip.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les utilisateurs.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetByIdWithLicensesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.Licenses)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.Licenses)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            throw new KeyNotFoundException($"Utilisateur {userId} introuvable.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateUserLicensesAsync(Guid userId, IList<Core.Enums.LicenseType> licenseTypes, CancellationToken cancellationToken = default)
    {
        // Supprimer les permis existants
        var existingLicenses = await _context.Set<Core.Entities.UserLicense>()
            .Where(ul => ul.UserId == userId)
            .ToListAsync(cancellationToken);

        _context.Set<Core.Entities.UserLicense>().RemoveRange(existingLicenses);

        // Ajouter les nouveaux permis
        foreach (var licenseType in licenseTypes)
        {
            var userLicense = Core.Entities.UserLicense.Create(userId, licenseType);
            await _context.Set<Core.Entities.UserLicense>().AddAsync(userLicense, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
