using SyncTrip.Core.Enums;

namespace SyncTrip.Core.Entities;

/// <summary>
/// Représente un permis de conduire détenu par un utilisateur.
/// Table de liaison entre User et LicenseType avec clé composite.
/// </summary>
public class UserLicense
{
    /// <summary>
    /// Identifiant de l'utilisateur.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Utilisateur propriétaire du permis.
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Type de permis (AM, A1, A2, A, B, BE, C, CE, D, DE).
    /// </summary>
    public LicenseType LicenseType { get; private set; }

    /// <summary>
    /// Date d'ajout du permis au profil.
    /// </summary>
    public DateTime AddedAt { get; private set; }

    // Constructeur privé pour EF Core
    private UserLicense()
    {
    }

    /// <summary>
    /// Factory method pour créer une nouvelle association utilisateur-permis.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="licenseType">Type de permis.</param>
    /// <returns>Nouvelle instance de UserLicense.</returns>
    /// <exception cref="ArgumentException">Si l'identifiant utilisateur est invalide.</exception>
    public static UserLicense Create(Guid userId, LicenseType licenseType)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("L'identifiant utilisateur est invalide.", nameof(userId));

        return new UserLicense
        {
            UserId = userId,
            LicenseType = licenseType,
            AddedAt = DateTime.UtcNow
        };
    }
}
