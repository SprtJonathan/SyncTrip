namespace SyncTrip.Shared.DTOs.Voting;

/// <summary>
/// DTO représentant une proposition d'arrêt.
/// </summary>
public class StopProposalDto
{
    /// <summary>
    /// Identifiant unique de la proposition.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifiant du voyage.
    /// </summary>
    public Guid TripId { get; init; }

    /// <summary>
    /// Identifiant de l'utilisateur ayant proposé.
    /// </summary>
    public Guid ProposedByUserId { get; init; }

    /// <summary>
    /// Nom d'utilisateur du proposeur.
    /// </summary>
    public string ProposedByUsername { get; init; } = string.Empty;

    /// <summary>
    /// Type d'arrêt (1=Fuel, 2=Break, 3=Food, 4=Photo).
    /// </summary>
    public int StopType { get; init; }

    /// <summary>
    /// Latitude de l'arrêt proposé.
    /// </summary>
    public double Latitude { get; init; }

    /// <summary>
    /// Longitude de l'arrêt proposé.
    /// </summary>
    public double Longitude { get; init; }

    /// <summary>
    /// Nom du lieu.
    /// </summary>
    public string LocationName { get; init; } = string.Empty;

    /// <summary>
    /// Statut (1=Pending, 2=Accepted, 3=Rejected).
    /// </summary>
    public int Status { get; init; }

    /// <summary>
    /// Date/heure de création.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Date/heure d'expiration du vote.
    /// </summary>
    public DateTime ExpiresAt { get; init; }

    /// <summary>
    /// Date/heure de résolution.
    /// </summary>
    public DateTime? ResolvedAt { get; init; }

    /// <summary>
    /// Nombre de votes OUI.
    /// </summary>
    public int YesCount { get; init; }

    /// <summary>
    /// Nombre de votes NON.
    /// </summary>
    public int NoCount { get; init; }

    /// <summary>
    /// Identifiant du waypoint créé si accepté.
    /// </summary>
    public Guid? CreatedWaypointId { get; init; }

    /// <summary>
    /// Liste des votes.
    /// </summary>
    public List<VoteDto> Votes { get; init; } = new();
}
