using System.ComponentModel.DataAnnotations;

namespace SyncTrip.Api.Application.DTOs.Convoys;

/// <summary>
/// Requête de création de convoi
/// </summary>
public class CreateConvoyRequest
{
    [MaxLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    public string? Name { get; set; }

    [MaxLength(100, ErrorMessage = "Le nom du véhicule ne peut pas dépasser 100 caractères")]
    public string? VehicleName { get; set; }
}
