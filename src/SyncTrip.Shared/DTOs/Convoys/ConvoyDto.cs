namespace SyncTrip.Shared.DTOs.Convoys;

/// <summary>
/// DTO représentant un convoi (vue résumée).
/// </summary>
public class ConvoyDto
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
    /// Nom d'utilisateur du leader.
    /// </summary>
    public string LeaderUsername { get; init; } = string.Empty;

    /// <summary>
    /// Indique si le convoi est privé.
    /// </summary>
    public bool IsPrivate { get; init; }

    /// <summary>
    /// Nombre de membres dans le convoi.
    /// </summary>
    public int MemberCount { get; init; }

    /// <summary>
    /// Date de création du convoi.
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
