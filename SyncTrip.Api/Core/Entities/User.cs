using System.ComponentModel.DataAnnotations;

namespace SyncTrip.Api.Core.Entities;

/// <summary>
/// Utilisateur de l'application
/// </summary>
public class User : BaseEntity
{

    /// <summary>
    /// Email unique de l'utilisateur (utilisé pour les magic links)
    /// </summary>
    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nom d'affichage de l'utilisateur
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Numéro de téléphone (optionnel, pour 2FA SMS)
    /// </summary>
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// URL de l'avatar/photo de profil (optionnel)
    /// </summary>
    [MaxLength(500)]
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Bio/description courte du profil (optionnel)
    /// </summary>
    [MaxLength(500)]
    public string? Bio { get; set; }

    /// <summary>
    /// 2FA activé ?
    /// </summary>
    public bool TwoFactorEnabled { get; set; } = false;

    /// <summary>
    /// Dernière connexion
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Compte actif ?
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<ConvoyParticipant> ConvoyParticipations { get; set; } = new List<ConvoyParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<LocationHistory> LocationHistory { get; set; } = new List<LocationHistory>();
    public ICollection<MagicLinkToken> MagicLinkTokens { get; set; } = new List<MagicLinkToken>();
}
