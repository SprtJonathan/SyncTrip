using System.ComponentModel.DataAnnotations;

namespace SyncTrip.Api.Application.DTOs.Auth;

/// <summary>
/// Requête de connexion par magic link
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "L'email est obligatoire")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    public string Email { get; set; } = string.Empty;
}
