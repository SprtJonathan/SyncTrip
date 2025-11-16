using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncTrip.Api.Core.Entities;

/// <summary>
/// Token pour l'authentification passwordless par magic link
/// </summary>
public class MagicLinkToken : BaseEntity
{

    /// <summary>
    /// ID de l'utilisateur
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Token unique (généré aléatoirement)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Date/heure d'expiration (15 minutes après création)
    /// </summary>
    [Required]
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Token utilisé ?
    /// </summary>
    public bool IsUsed { get; set; } = false;

    /// <summary>
    /// Date/heure d'utilisation (si utilisé)
    /// </summary>
    public DateTime? UsedAt { get; set; }

    /// <summary>
    /// Adresse IP de la demande
    /// </summary>
    [MaxLength(45)] // IPv6 max length
    public string? RequestIpAddress { get; set; }

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
