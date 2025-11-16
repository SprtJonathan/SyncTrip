using System.ComponentModel.DataAnnotations;

namespace SyncTrip.Api.Application.DTOs.Convoys;

/// <summary>
/// Requête de mise à jour de convoi
/// </summary>
public class UpdateConvoyRequest
{
    [MaxLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    public string? Name { get; set; }
}
