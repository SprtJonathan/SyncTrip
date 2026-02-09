namespace SyncTrip.Shared.DTOs.Convoys;

/// <summary>
/// DTO représentant les détails complets d'un convoi avec ses membres.
/// </summary>
public class ConvoyDetailsDto
{
    /// <summary>
    /// Identifiant unique du convoi.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Code d'accès au convoi.
    /// </summary>
    public string JoinCode { get; init; } = string.Empty;

    /// <summary>
    /// Identifiant du leader.
    /// </summary>
    public Guid LeaderUserId { get; init; }

    /// <summary>
    /// Nom d'utilisateur du leader.
    /// </summary>
    public string LeaderUsername { get; init; } = string.Empty;

    /// <summary>
    /// Indique si le convoi est privé.
    /// </summary>
    public bool IsPrivate { get; init; }

    /// <summary>
    /// Date de création du convoi.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Liste des membres du convoi.
    /// </summary>
    public List<ConvoyMemberDto> Members { get; init; } = new();
}
