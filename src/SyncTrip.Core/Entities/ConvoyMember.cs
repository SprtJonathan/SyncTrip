using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;

namespace SyncTrip.Core.Entities;

/// <summary>
/// Représente la participation d'un utilisateur à un convoi.
/// </summary>
public class ConvoyMember
{
    /// <summary>
    /// Identifiant du convoi.
    /// </summary>
    public Guid ConvoyId { get; private set; }

    /// <summary>
    /// Convoi associé.
    /// </summary>
    public Convoy Convoy { get; private set; } = null!;

    /// <summary>
    /// Identifiant de l'utilisateur membre.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Utilisateur membre.
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Identifiant du véhicule utilisé dans ce convoi.
    /// </summary>
    public Guid VehicleId { get; private set; }

    /// <summary>
    /// Véhicule utilisé dans ce convoi.
    /// </summary>
    public Vehicle Vehicle { get; private set; } = null!;

    /// <summary>
    /// Rôle du membre dans le convoi (Leader ou Member).
    /// </summary>
    public ConvoyRole Role { get; private set; }

    /// <summary>
    /// Date d'entrée dans le convoi.
    /// </summary>
    public DateTime JoinedAt { get; private set; }

    // Constructeur privé pour EF Core
    private ConvoyMember()
    {
    }

    /// <summary>
    /// Factory method pour créer un membre de convoi.
    /// </summary>
    /// <param name="convoyId">Identifiant du convoi.</param>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="vehicleId">Identifiant du véhicule.</param>
    /// <param name="role">Rôle dans le convoi.</param>
    /// <returns>Nouvelle instance de ConvoyMember.</returns>
    public static ConvoyMember Create(Guid convoyId, Guid userId, Guid vehicleId, ConvoyRole role)
    {
        if (convoyId == Guid.Empty)
            throw new ArgumentException("L'identifiant du convoi est invalide.", nameof(convoyId));

        if (userId == Guid.Empty)
            throw new ArgumentException("L'identifiant utilisateur est invalide.", nameof(userId));

        if (vehicleId == Guid.Empty)
            throw new DomainException("Un véhicule est requis pour rejoindre un convoi.");

        return new ConvoyMember
        {
            ConvoyId = convoyId,
            UserId = userId,
            VehicleId = vehicleId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Promeut ce membre au rôle de Leader.
    /// </summary>
    public void PromoteToLeader()
    {
        Role = ConvoyRole.Leader;
    }

    /// <summary>
    /// Rétrograde ce membre au rôle de Member.
    /// </summary>
    public void DemoteToMember()
    {
        Role = ConvoyRole.Member;
    }
}
