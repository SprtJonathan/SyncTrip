namespace SyncTrip.Shared.DTOs.Voting;

/// <summary>
/// DTO représentant un vote sur une proposition d'arrêt.
/// </summary>
public class VoteDto
{
    /// <summary>
    /// Identifiant unique du vote.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifiant de l'utilisateur ayant voté.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Nom d'utilisateur du votant.
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// True pour OUI, False pour NON.
    /// </summary>
    public bool IsYes { get; init; }

    /// <summary>
    /// Date/heure du vote.
    /// </summary>
    public DateTime VotedAt { get; init; }
}
