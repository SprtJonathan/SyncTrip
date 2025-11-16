using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Core.Entities;

/// <summary>
/// Table de jonction User-Convoy avec rôle
/// </summary>
public class ConvoyParticipant : BaseEntity
{

    /// <summary>
    /// ID du convoi
    /// </summary>
    [Required]
    public Guid ConvoyId { get; set; }

    /// <summary>
    /// ID de l'utilisateur
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Rôle du participant dans le convoi
    /// </summary>
    public ConvoyRole Role { get; set; } = ConvoyRole.Member;

    /// <summary>
    /// Nom du véhicule (optionnel, ex: "Peugeot 308 grise")
    /// </summary>
    [MaxLength(100)]
    public string? VehicleName { get; set; }

    /// <summary>
    /// Date de jointure au convoi
    /// </summary>
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date de sortie du convoi (si parti)
    /// </summary>
    public DateTime? LeftAt { get; set; }

    /// <summary>
    /// Participant actif dans le convoi ?
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    [ForeignKey(nameof(ConvoyId))]
    public Convoy Convoy { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
