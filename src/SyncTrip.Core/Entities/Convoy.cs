using System.Security.Cryptography;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;

namespace SyncTrip.Core.Entities;

/// <summary>
/// Représente un convoi de véhicules.
/// </summary>
public class Convoy
{
    /// <summary>
    /// Identifiant unique du convoi.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Code d'accès au convoi (6 caractères, ex: K9P2XL).
    /// </summary>
    public string JoinCode { get; private set; } = string.Empty;

    /// <summary>
    /// Identifiant du leader (créateur) du convoi.
    /// </summary>
    public Guid LeaderUserId { get; private set; }

    /// <summary>
    /// Indique si le convoi est privé (nécessite validation du leader pour rejoindre).
    /// </summary>
    public bool IsPrivate { get; private set; }

    /// <summary>
    /// Date de création du convoi.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Collection des membres du convoi.
    /// </summary>
    public ICollection<ConvoyMember> Members { get; private set; } = new List<ConvoyMember>();

    /// <summary>
    /// Caractères autorisés pour le code de convoi (sans ambiguïté visuelle : 0/O, 1/I/L exclus).
    /// </summary>
    private const string CodeCharacters = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

    /// <summary>
    /// Longueur du code de convoi.
    /// </summary>
    private const int CodeLength = 6;

    // Constructeur privé pour EF Core
    private Convoy()
    {
    }

    /// <summary>
    /// Factory method pour créer un nouveau convoi.
    /// </summary>
    /// <param name="leaderUserId">Identifiant du créateur (leader).</param>
    /// <param name="leaderVehicleId">Identifiant du véhicule du leader.</param>
    /// <param name="isPrivate">Convoi privé ou ouvert.</param>
    /// <returns>Nouvelle instance de Convoy avec le leader comme premier membre.</returns>
    public static Convoy Create(Guid leaderUserId, Guid leaderVehicleId, bool isPrivate)
    {
        if (leaderUserId == Guid.Empty)
            throw new ArgumentException("L'identifiant du leader est invalide.", nameof(leaderUserId));

        if (leaderVehicleId == Guid.Empty)
            throw new DomainException("Un véhicule est requis pour créer un convoi.");

        var convoy = new Convoy
        {
            Id = Guid.NewGuid(),
            JoinCode = GenerateJoinCode(),
            LeaderUserId = leaderUserId,
            IsPrivate = isPrivate,
            CreatedAt = DateTime.UtcNow
        };

        // Le créateur est automatiquement membre avec le rôle Leader
        var leaderMember = ConvoyMember.Create(convoy.Id, leaderUserId, leaderVehicleId, ConvoyRole.Leader);
        convoy.Members.Add(leaderMember);

        return convoy;
    }

    /// <summary>
    /// Ajoute un nouveau membre au convoi.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="vehicleId">Identifiant du véhicule.</param>
    /// <exception cref="DomainException">Si l'utilisateur est déjà membre.</exception>
    public ConvoyMember AddMember(Guid userId, Guid vehicleId)
    {
        if (Members.Any(m => m.UserId == userId))
            throw new DomainException("Cet utilisateur est déjà membre du convoi.");

        var member = ConvoyMember.Create(Id, userId, vehicleId, ConvoyRole.Member);
        Members.Add(member);
        return member;
    }

    /// <summary>
    /// Retire un membre du convoi.
    /// </summary>
    /// <param name="userId">Identifiant du membre à retirer.</param>
    /// <exception cref="DomainException">Si le leader tente de se retirer ou si le membre n'existe pas.</exception>
    public void RemoveMember(Guid userId)
    {
        if (userId == LeaderUserId)
            throw new DomainException("Le leader ne peut pas quitter le convoi. Transférez le leadership d'abord.");

        var member = Members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
            throw new DomainException("Cet utilisateur n'est pas membre du convoi.");

        Members.Remove(member);
    }

    /// <summary>
    /// Exclut un membre du convoi (action réservée au leader).
    /// </summary>
    /// <param name="requestingUserId">Identifiant de l'utilisateur faisant la demande.</param>
    /// <param name="targetUserId">Identifiant du membre à exclure.</param>
    /// <exception cref="DomainException">Si le demandeur n'est pas leader ou si la cible est invalide.</exception>
    public void KickMember(Guid requestingUserId, Guid targetUserId)
    {
        EnsureIsLeader(requestingUserId);

        if (targetUserId == LeaderUserId)
            throw new DomainException("Le leader ne peut pas s'exclure lui-même.");

        var member = Members.FirstOrDefault(m => m.UserId == targetUserId);
        if (member == null)
            throw new DomainException("Cet utilisateur n'est pas membre du convoi.");

        Members.Remove(member);
    }

    /// <summary>
    /// Transfère le leadership à un autre membre.
    /// </summary>
    /// <param name="requestingUserId">Leader actuel.</param>
    /// <param name="newLeaderUserId">Nouveau leader.</param>
    /// <exception cref="DomainException">Si le demandeur n'est pas leader ou si la cible est invalide.</exception>
    public void TransferLeadership(Guid requestingUserId, Guid newLeaderUserId)
    {
        EnsureIsLeader(requestingUserId);

        if (newLeaderUserId == LeaderUserId)
            throw new DomainException("Cet utilisateur est déjà le leader.");

        var newLeader = Members.FirstOrDefault(m => m.UserId == newLeaderUserId);
        if (newLeader == null)
            throw new DomainException("Le nouveau leader doit être membre du convoi.");

        var currentLeader = Members.FirstOrDefault(m => m.UserId == LeaderUserId);
        currentLeader?.DemoteToMember();

        newLeader.PromoteToLeader();
        LeaderUserId = newLeaderUserId;
    }

    /// <summary>
    /// Vérifie qu'un utilisateur est le leader du convoi.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur à vérifier.</param>
    /// <exception cref="DomainException">Si l'utilisateur n'est pas le leader.</exception>
    public void EnsureIsLeader(Guid userId)
    {
        if (userId != LeaderUserId)
            throw new DomainException("Seul le leader peut effectuer cette action.");
    }

    /// <summary>
    /// Vérifie si un utilisateur est membre du convoi.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <returns>True si l'utilisateur est membre.</returns>
    public bool IsMember(Guid userId)
    {
        return Members.Any(m => m.UserId == userId);
    }

    /// <summary>
    /// Génère un code d'accès aléatoire de 6 caractères.
    /// Utilise des caractères sans ambiguïté visuelle.
    /// </summary>
    /// <returns>Code de 6 caractères (ex: K9P2XL).</returns>
    private static string GenerateJoinCode()
    {
        var chars = new char[CodeLength];
        for (int i = 0; i < CodeLength; i++)
        {
            chars[i] = CodeCharacters[RandomNumberGenerator.GetInt32(CodeCharacters.Length)];
        }
        return new string(chars);
    }
}
