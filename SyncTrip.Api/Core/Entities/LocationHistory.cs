using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncTrip.Api.Core.Entities;

/// <summary>
/// Historique des positions GPS des participants
/// </summary>
public class LocationHistory : BaseEntity
{

    /// <summary>
    /// ID du trip
    /// </summary>
    [Required]
    public Guid TripId { get; set; }

    /// <summary>
    /// ID de l'utilisateur
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Latitude
    /// </summary>
    [Required]
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude
    /// </summary>
    [Required]
    public double Longitude { get; set; }

    /// <summary>
    /// Altitude (optionnel, en mètres)
    /// </summary>
    public double? Altitude { get; set; }

    /// <summary>
    /// Vitesse (optionnel, en km/h)
    /// </summary>
    public double? Speed { get; set; }

    /// <summary>
    /// Cap / Direction (optionnel, en degrés 0-360)
    /// </summary>
    public double? Heading { get; set; }

    /// <summary>
    /// Précision de la position (optionnel, en mètres)
    /// </summary>
    public double? Accuracy { get; set; }

    /// <summary>
    /// Timestamp de capture de la position
    /// </summary>
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(TripId))]
    public Trip Trip { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
