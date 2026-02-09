namespace SyncTrip.Shared.DTOs.Convoys;

/// <summary>
/// DTO représentant un membre d'un convoi.
/// </summary>
public class ConvoyMemberDto
{
    /// <summary>
    /// Identifiant de l'utilisateur.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Nom d'utilisateur (pseudo).
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// URL de l'avatar (facultatif).
    /// </summary>
    public string? AvatarUrl { get; init; }

    /// <summary>
    /// Nom de la marque du véhicule.
    /// </summary>
    public string VehicleBrand { get; init; } = string.Empty;

    /// <summary>
    /// Modèle du véhicule.
    /// </summary>
    public string VehicleModel { get; init; } = string.Empty;

    /// <summary>
    /// Couleur du véhicule (facultatif).
    /// </summary>
    public string? VehicleColor { get; init; }

    /// <summary>
    /// Rôle dans le convoi (1=Member, 2=Leader).
    /// </summary>
    public int Role { get; init; }

    /// <summary>
    /// Date d'entrée dans le convoi.
    /// </summary>
    public DateTime JoinedAt { get; init; }
}
