using System.ComponentModel.DataAnnotations;

namespace SyncTrip.Api.Application.DTOs.Waypoints;

/// <summary>
/// Requête de création de waypoint
/// </summary>
public class CreateWaypointRequest
{
    [Required(ErrorMessage = "Le nom est obligatoire")]
    [MaxLength(200, ErrorMessage = "Le nom ne peut pas dépasser 200 caractères")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "La latitude est obligatoire")]
    [Range(-90, 90, ErrorMessage = "La latitude doit être entre -90 et 90")]
    public double Latitude { get; set; }

    [Required(ErrorMessage = "La longitude est obligatoire")]
    [Range(-180, 180, ErrorMessage = "La longitude doit être entre -180 et 180")]
    public double Longitude { get; set; }
}
