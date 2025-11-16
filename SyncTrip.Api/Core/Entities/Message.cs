using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Core.Entities;

/// <summary>
/// Message dans le chat d'un convoi
/// </summary>
public class Message : BaseEntity
{

    /// <summary>
    /// ID du convoi
    /// </summary>
    [Required]
    public Guid ConvoyId { get; set; }

    /// <summary>
    /// ID de l'utilisateur (null si message système)
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Type de message
    /// </summary>
    public MessageType Type { get; set; } = MessageType.User;

    /// <summary>
    /// Contenu du message
    /// </summary>
    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Date/heure d'envoi
    /// </summary>
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Message supprimé ?
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Date de suppression (si supprimé)
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    [ForeignKey(nameof(ConvoyId))]
    public Convoy Convoy { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}
