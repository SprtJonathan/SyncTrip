using System.ComponentModel.DataAnnotations;

namespace SyncTrip.Api.Application.DTOs.Convoys;

/// <summary>
/// Requête pour rejoindre un convoi
/// </summary>
public class JoinConvoyRequest
{
    [Required(ErrorMessage = "Le code du convoi est obligatoire")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Le code doit contenir exactement 6 caractères")]
    [RegularExpression(@"^[A-Za-z0-9]{6}$", ErrorMessage = "Le code doit contenir uniquement des lettres et chiffres")]
    public string Code { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "Le nom du véhicule ne peut pas dépasser 100 caractères")]
    public string? VehicleName { get; set; }
}
