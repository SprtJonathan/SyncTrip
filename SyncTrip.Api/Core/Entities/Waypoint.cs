using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncTrip.Api.Core.Entities;

/// <summary>
/// Point d'intérêt / étape d'un voyage
/// </summary>
public class Waypoint : BaseEntity
{

    /// <summary>
    /// ID du trip
    /// </summary>
    [Required]
    public Guid TripId { get; set; }

    /// <summary>
    /// Nom du waypoint
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description (optionnel)
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

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
    /// Ordre d'affichage (0 = premier)
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Waypoint atteint ?
    /// </summary>
    public bool IsReached { get; set; } = false;

    /// <summary>
    /// Date/heure d'atteinte (si atteint)
    /// </summary>
    public DateTime? ReachedAt { get; set; }

    // Navigation properties
    [ForeignKey(nameof(TripId))]
    public Trip Trip { get; set; } = null!;
}
