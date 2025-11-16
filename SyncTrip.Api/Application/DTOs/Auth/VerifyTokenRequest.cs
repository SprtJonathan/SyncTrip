using System.ComponentModel.DataAnnotations;

namespace SyncTrip.Api.Application.DTOs.Auth;

/// <summary>
/// Requête de vérification de token magic link
/// </summary>
public class VerifyTokenRequest
{
    [Required(ErrorMessage = "Le token est obligatoire")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Code OTP 2FA (optionnel)
    /// </summary>
    public string? OtpCode { get; set; }
}
