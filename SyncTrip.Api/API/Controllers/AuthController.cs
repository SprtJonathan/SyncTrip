using Microsoft.AspNetCore.Mvc;
using SyncTrip.Api.Application.DTOs.Auth;
using SyncTrip.Api.Application.DTOs.Common;
using SyncTrip.Api.Core.Interfaces;

namespace SyncTrip.Api.API.Controllers;

/// <summary>
/// Controller d'authentification (magic link passwordless)
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Inscription d'un nouvel utilisateur
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _authService.RegisterAsync(request, cancellationToken);
            return Ok(ApiResponse<UserDto>.SuccessResult(user, "Utilisateur créé avec succès"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'inscription");
            return BadRequest(ApiResponse<UserDto>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Demande d'envoi d'un magic link par email
    /// </summary>
    [HttpPost("send-magic-link")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendMagicLink([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _authService.SendMagicLinkAsync(request.Email, cancellationToken);
            return Ok(ApiResponse<string>.SuccessResult("Magic link envoyé", "Vérifiez votre email"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi du magic link");
            return BadRequest(ApiResponse<string>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Vérification du magic link et obtention des tokens JWT
    /// </summary>
    [HttpPost("verify-magic-link")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VerifyMagicLink([FromBody] VerifyTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var authResponse = await _authService.VerifyMagicLinkAsync(request.Token, cancellationToken);
            return Ok(ApiResponse<AuthResponse>.SuccessResult(authResponse, "Authentification réussie"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<AuthResponse>.FailureResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification du magic link");
            return BadRequest(ApiResponse<AuthResponse>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Rafraîchissement du token JWT à l'aide d'un refresh token
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var authResponse = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
            return Ok(ApiResponse<AuthResponse>.SuccessResult(authResponse, "Token rafraîchi avec succès"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<AuthResponse>.FailureResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du rafraîchissement du token");
            return BadRequest(ApiResponse<AuthResponse>.FailureResult(ex.Message));
        }
    }
}
