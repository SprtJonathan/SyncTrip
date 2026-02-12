using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;

namespace SyncTrip.Core.Entities;

/// <summary>
/// Représente une proposition d'arrêt soumise au vote des membres d'un voyage.
/// </summary>
public class StopProposal
{
    /// <summary>
    /// Identifiant unique de la proposition.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identifiant du voyage associé.
    /// </summary>
    public Guid TripId { get; private set; }

    /// <summary>
    /// Voyage associé.
    /// </summary>
    public Trip Trip { get; private set; } = null!;

    /// <summary>
    /// Identifiant de l'utilisateur ayant proposé l'arrêt.
    /// </summary>
    public Guid ProposedByUserId { get; private set; }

    /// <summary>
    /// Utilisateur ayant proposé l'arrêt.
    /// </summary>
    public User ProposedByUser { get; private set; } = null!;

    /// <summary>
    /// Type d'arrêt proposé.
    /// </summary>
    public StopType StopType { get; private set; }

    /// <summary>
    /// Latitude de l'arrêt proposé.
    /// </summary>
    public double Latitude { get; private set; }

    /// <summary>
    /// Longitude de l'arrêt proposé.
    /// </summary>
    public double Longitude { get; private set; }

    /// <summary>
    /// Nom du lieu de l'arrêt proposé.
    /// </summary>
    public string LocationName { get; private set; } = string.Empty;

    /// <summary>
    /// Statut actuel de la proposition.
    /// </summary>
    public ProposalStatus Status { get; private set; }

    /// <summary>
    /// Date/heure de création de la proposition.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date/heure d'expiration du vote (30 secondes après création).
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Date/heure de résolution de la proposition.
    /// </summary>
    public DateTime? ResolvedAt { get; private set; }

    /// <summary>
    /// Identifiant du waypoint créé si la proposition est acceptée.
    /// </summary>
    public Guid? CreatedWaypointId { get; private set; }

    /// <summary>
    /// Collection des votes sur cette proposition.
    /// </summary>
    public ICollection<Vote> Votes { get; private set; } = new List<Vote>();

    // Constructeur privé pour EF Core
    private StopProposal()
    {
    }

    /// <summary>
    /// Factory method pour créer une nouvelle proposition d'arrêt.
    /// </summary>
    /// <param name="tripId">Identifiant du voyage.</param>
    /// <param name="proposedByUserId">Identifiant du proposeur.</param>
    /// <param name="stopType">Type d'arrêt.</param>
    /// <param name="latitude">Latitude de l'arrêt.</param>
    /// <param name="longitude">Longitude de l'arrêt.</param>
    /// <param name="locationName">Nom du lieu.</param>
    /// <returns>Nouvelle instance de StopProposal.</returns>
    public static StopProposal Create(
        Guid tripId,
        Guid proposedByUserId,
        StopType stopType,
        double latitude,
        double longitude,
        string locationName)
    {
        if (tripId == Guid.Empty)
            throw new ArgumentException("L'identifiant du voyage est invalide.", nameof(tripId));

        if (proposedByUserId == Guid.Empty)
            throw new ArgumentException("L'identifiant de l'utilisateur est invalide.", nameof(proposedByUserId));

        if (string.IsNullOrWhiteSpace(locationName))
            throw new DomainException("Le nom du lieu est obligatoire.");

        if (latitude < -90 || latitude > 90)
            throw new DomainException("La latitude doit être comprise entre -90 et 90.");

        if (longitude < -180 || longitude > 180)
            throw new DomainException("La longitude doit être comprise entre -180 et 180.");

        var now = DateTime.UtcNow;

        return new StopProposal
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            ProposedByUserId = proposedByUserId,
            StopType = stopType,
            Latitude = latitude,
            Longitude = longitude,
            LocationName = locationName,
            Status = ProposalStatus.Pending,
            CreatedAt = now,
            ExpiresAt = now.AddSeconds(30),
            ResolvedAt = null,
            CreatedWaypointId = null
        };
    }

    /// <summary>
    /// Enregistre un vote sur cette proposition.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur votant.</param>
    /// <param name="isYes">True pour OUI, False pour NON.</param>
    /// <exception cref="DomainException">Si la proposition n'est pas en attente ou si l'utilisateur a déjà voté.</exception>
    public void CastVote(Guid userId, bool isYes)
    {
        if (Status != ProposalStatus.Pending)
            throw new DomainException("Cette proposition n'est plus en attente de votes.");

        if (Votes.Any(v => v.UserId == userId))
            throw new DomainException("Cet utilisateur a déjà voté sur cette proposition.");

        var vote = Vote.Create(Id, userId, isYes);
        Votes.Add(vote);
    }

    /// <summary>
    /// Résout la proposition selon la règle du silence.
    /// Si la majorité absolue vote NON, la proposition est rejetée.
    /// Sinon (silence = consentement), la proposition est acceptée.
    /// </summary>
    /// <param name="totalMemberCount">Nombre total de membres dans le convoi.</param>
    /// <exception cref="DomainException">Si la proposition n'est pas en attente.</exception>
    public void Resolve(int totalMemberCount)
    {
        if (Status != ProposalStatus.Pending)
            throw new DomainException("Cette proposition a déjà été résolue.");

        var noCount = Votes.Count(v => !v.IsYes);

        // Majorité absolue de NON requis pour rejeter
        Status = noCount > totalMemberCount / 2.0
            ? ProposalStatus.Rejected
            : ProposalStatus.Accepted;

        ResolvedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Vérifie si tous les membres ont voté.
    /// </summary>
    /// <param name="totalMemberCount">Nombre total de membres dans le convoi.</param>
    /// <returns>True si tous les membres ont voté.</returns>
    public bool AllMembersVoted(int totalMemberCount)
    {
        return Votes.Count >= totalMemberCount;
    }

    /// <summary>
    /// Associe le waypoint créé suite à l'acceptation de la proposition.
    /// </summary>
    /// <param name="waypointId">Identifiant du waypoint créé.</param>
    /// <exception cref="DomainException">Si la proposition n'est pas acceptée.</exception>
    public void SetCreatedWaypoint(Guid waypointId)
    {
        if (Status != ProposalStatus.Accepted)
            throw new DomainException("Seule une proposition acceptée peut être liée à un waypoint.");

        if (waypointId == Guid.Empty)
            throw new ArgumentException("L'identifiant du waypoint est invalide.", nameof(waypointId));

        CreatedWaypointId = waypointId;
    }
}
