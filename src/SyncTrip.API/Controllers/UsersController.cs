using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SyncTrip.Application.Users.Commands;
using SyncTrip.Application.Users.Queries;
using SyncTrip.Shared.DTOs.Users;

namespace SyncTrip.API.Controllers;

/// <summary>
/// Contrôleur pour la gestion du profil utilisateur.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    /// <summary>
    /// Initialise une nouvelle instance du contrôleur utilisateurs.
    /// </summary>
    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Récupère le profil de l'utilisateur connecté.
    /// </summary>
    /// <returns>Profil utilisateur complet avec permis de conduire.</returns>
    [HttpGet("me")]
    [ProducesResponseType<UserProfileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = GetCurrentUserId();

        try
        {
            var query = new GetUserProfileQuery(userId);
            var profile = await _mediator.Send(query);
            return Ok(profile);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Profil introuvable pour l'utilisateur {UserId}", userId);
            return NotFound(new { Message = "Profil utilisateur introuvable" });
        }
    }

    /// <summary>
    /// Met à jour le profil de l'utilisateur connecté.
    /// </summary>
    /// <param name="request">Données de mise à jour du profil.</param>
    /// <returns>Confirmation de mise à jour.</returns>
    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserProfileRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new UpdateUserProfileCommand
            {
                UserId = userId,
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate,
                AvatarUrl = request.AvatarUrl,
                LicenseTypes = request.LicenseTypes
            };

            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Utilisateur introuvable lors de la mise à jour : {UserId}", userId);
            return NotFound(new { Message = "Utilisateur introuvable" });
        }
        catch (Core.Exceptions.DomainException ex)
        {
            _logger.LogWarning("Erreur de validation lors de la mise à jour du profil : {Message}", ex.Message);
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Récupère l'ID de l'utilisateur connecté depuis le JWT.
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié ou ID invalide");
        }

        return userId;
    }
}
