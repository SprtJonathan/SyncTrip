using System.ComponentModel.DataAnnotations;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Application.DTOs.Trips;

/// <summary>
/// Requête de création de trip
/// </summary>
public class CreateTripRequest
{
    [Required(ErrorMessage = "L'ID du convoi est obligatoire")]
    public Guid ConvoyId { get; set; }

    [MaxLength(200, ErrorMessage = "Le nom ne peut pas dépasser 200 caractères")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "La destination est obligatoire")]
    [MaxLength(500, ErrorMessage = "La destination ne peut pas dépasser 500 caractères")]
    public string Destination { get; set; } = string.Empty;

    [Required(ErrorMessage = "La latitude de destination est obligatoire")]
    [Range(-90, 90, ErrorMessage = "La latitude doit être entre -90 et 90")]
    public double DestinationLatitude { get; set; }

    [Required(ErrorMessage = "La longitude de destination est obligatoire")]
    [Range(-180, 180, ErrorMessage = "La longitude doit être entre -180 et 180")]
    public double DestinationLongitude { get; set; }

    public RoutePreference RoutePreference { get; set; } = RoutePreference.Fastest;

    public DateTime? PlannedDepartureTime { get; set; }
    public DateTime? PlannedArrivalTime { get; set; }
}
