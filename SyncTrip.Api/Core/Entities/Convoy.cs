using System.ComponentModel.DataAnnotations;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Core.Entities;

/// <summary>
/// Convoi - groupe persistant de véhicules
/// </summary>
public class Convoy : BaseEntity
{

    /// <summary>
    /// Code unique du convoi (6 caractères alphanumériques [A-Za-z0-9])
    /// </summary>
    [Required]
    [MaxLength(6)]
    [MinLength(6)]
    [RegularExpression(@"^[A-Za-z0-9]{6}$")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nom du convoi (optionnel)
    /// </summary>
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Statut du convoi
    /// </summary>
    public ConvoyStatus Status { get; set; } = ConvoyStatus.Active;

    /// <summary>
    /// Date d'archivage (si archivé)
    /// </summary>
    public DateTime? ArchivedAt { get; set; }

    /// <summary>
    /// Date de suppression logique (si supprimé)
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public ICollection<ConvoyParticipant> Participants { get; set; } = new List<ConvoyParticipant>();
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
