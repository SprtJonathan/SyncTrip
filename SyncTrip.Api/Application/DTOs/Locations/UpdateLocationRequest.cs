using System.ComponentModel.DataAnnotations;

namespace SyncTrip.Api.Application.DTOs.Locations;

/// <summary>
/// Requête de mise à jour de position GPS
/// </summary>
public class UpdateLocationRequest
{
    [Required(ErrorMessage = "La latitude est obligatoire")]
    [Range(-90, 90, ErrorMessage = "La latitude doit être entre -90 et 90")]
    public double Latitude { get; set; }

    [Required(ErrorMessage = "La longitude est obligatoire")]
    [Range(-180, 180, ErrorMessage = "La longitude doit être entre -180 et 180")]
    public double Longitude { get; set; }

    [Range(0, 100000, ErrorMessage = "L'altitude doit être entre 0 et 100000 mètres")]
    public double? Altitude { get; set; }

    [Range(0, 500, ErrorMessage = "La vitesse doit être entre 0 et 500 km/h")]
    public double? Speed { get; set; }

    [Range(0, 360, ErrorMessage = "Le cap doit être entre 0 et 360 degrés")]
    public double? Heading { get; set; }

    [Range(0, 1000, ErrorMessage = "La précision doit être entre 0 et 1000 mètres")]
    public double? Accuracy { get; set; }
}
