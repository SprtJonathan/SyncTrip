using System.ComponentModel.DataAnnotations;

namespace SyncTrip.Api.Application.DTOs.Auth;

/// <summary>
/// Requête d'inscription
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "L'email est obligatoire")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom d'affichage est obligatoire")]
    [MinLength(2, ErrorMessage = "Le nom doit contenir au moins 2 caractères")]
    [MaxLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    public string DisplayName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Format de téléphone invalide")]
    public string? PhoneNumber { get; set; }
}
