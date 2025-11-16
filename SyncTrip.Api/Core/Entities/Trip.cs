using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Core.Entities;

/// <summary>
/// Voyage d'un convoi vers une destination
/// </summary>
public class Trip : BaseEntity
{

    /// <summary>
    /// ID du convoi
    /// </summary>
    [Required]
    public Guid ConvoyId { get; set; }

    /// <summary>
    /// Nom du trip (optionnel)
    /// </summary>
    [MaxLength(200)]
    public string? Name { get; set; }

    /// <summary>
    /// Destination OBLIGATOIRE du trip
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Destination { get; set; } = string.Empty;

    /// <summary>
    /// Latitude de la destination
    /// </summary>
    [Required]
    public double DestinationLatitude { get; set; }

    /// <summary>
    /// Longitude de la destination
    /// </summary>
    [Required]
    public double DestinationLongitude { get; set; }

    /// <summary>
    /// Préférence de route
    /// </summary>
    public RoutePreference RoutePreference { get; set; } = RoutePreference.Fastest;

    /// <summary>
    /// Statut du trip
    /// </summary>
    public TripStatus Status { get; set; } = TripStatus.Planned;

    /// <summary>
    /// Date/heure de départ prévue (optionnel)
    /// </summary>
    public DateTime? PlannedDepartureTime { get; set; }

    /// <summary>
    /// Date/heure de départ réelle
    /// </summary>
    public DateTime? ActualDepartureTime { get; set; }

    /// <summary>
    /// Date/heure d'arrivée prévue (optionnel)
    /// </summary>
    public DateTime? PlannedArrivalTime { get; set; }

    /// <summary>
    /// Date/heure d'arrivée réelle
    /// </summary>
    public DateTime? ActualArrivalTime { get; set; }

    // Navigation properties
    [ForeignKey(nameof(ConvoyId))]
    public Convoy Convoy { get; set; } = null!;

    public ICollection<Waypoint> Waypoints { get; set; } = new List<Waypoint>();
    public ICollection<LocationHistory> LocationHistory { get; set; } = new List<LocationHistory>();
}
