using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SyncTrip.Application.Convoys.Commands;
using SyncTrip.Application.Convoys.Queries;
using SyncTrip.Core.Exceptions;
using SyncTrip.Shared.DTOs.Convoys;

namespace SyncTrip.API.Controllers;

/// <summary>
/// Contrôleur pour la gestion des convois.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ConvoysController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConvoysController> _logger;

    public ConvoysController(IMediator mediator, ILogger<ConvoysController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Crée un nouveau convoi. L'utilisateur connecté devient le leader.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<object>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateConvoy([FromBody] CreateConvoyRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new CreateConvoyCommand
            {
                UserId = userId,
                VehicleId = request.VehicleId,
                IsPrivate = request.IsPrivate
            };

            var convoyId = await _mediator.Send(command);

            _logger.LogInformation("Convoi créé : {ConvoyId} par {UserId}", convoyId, userId);

            return CreatedAtAction(
                nameof(GetConvoyByCode),
                new { code = convoyId },
                new { ConvoyId = convoyId }
            );
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Récupère les détails d'un convoi par son code d'accès.
    /// </summary>
    [HttpGet("{code}")]
    [ProducesResponseType<ConvoyDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConvoyByCode(string code)
    {
        var query = new GetConvoyByCodeQuery(code.ToUpperInvariant());
        var convoy = await _mediator.Send(query);

        if (convoy == null)
            return NotFound(new { Message = "Convoi introuvable." });

        return Ok(convoy);
    }

    /// <summary>
    /// Liste les convois de l'utilisateur connecté.
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType<IList<ConvoyDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyConvoys()
    {
        var userId = GetCurrentUserId();
        var query = new GetUserConvoysQuery(userId);
        var convoys = await _mediator.Send(query);
        return Ok(convoys);
    }

    /// <summary>
    /// Rejoint un convoi existant.
    /// </summary>
    [HttpPost("{code}/join")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> JoinConvoy(string code, [FromBody] JoinConvoyRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new JoinConvoyCommand
            {
                JoinCode = code.ToUpperInvariant(),
                UserId = userId,
                VehicleId = request.VehicleId
            };

            await _mediator.Send(command);

            _logger.LogInformation("Utilisateur {UserId} a rejoint le convoi {Code}", userId, code);

            return Ok(new { Message = "Vous avez rejoint le convoi." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Quitte un convoi.
    /// </summary>
    [HttpPost("{code}/leave")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LeaveConvoy(string code)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new LeaveConvoyCommand
            {
                JoinCode = code.ToUpperInvariant(),
                UserId = userId
            };

            await _mediator.Send(command);

            _logger.LogInformation("Utilisateur {UserId} a quitté le convoi {Code}", userId, code);

            return Ok(new { Message = "Vous avez quitté le convoi." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Exclut un membre du convoi (leader uniquement).
    /// </summary>
    [HttpPost("{code}/kick/{targetUserId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> KickMember(string code, Guid targetUserId)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new KickMemberCommand
            {
                JoinCode = code.ToUpperInvariant(),
                RequestingUserId = userId,
                TargetUserId = targetUserId
            };

            await _mediator.Send(command);

            _logger.LogInformation("Utilisateur {TargetUserId} exclu du convoi {Code} par {UserId}",
                targetUserId, code, userId);

            return Ok(new { Message = "Membre exclu du convoi." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Transfère le leadership à un autre membre (leader uniquement).
    /// </summary>
    [HttpPost("{code}/transfer/{newLeaderUserId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TransferLeadership(string code, Guid newLeaderUserId)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new TransferLeadershipCommand
            {
                JoinCode = code.ToUpperInvariant(),
                RequestingUserId = userId,
                NewLeaderUserId = newLeaderUserId
            };

            await _mediator.Send(command);

            _logger.LogInformation("Leadership du convoi {Code} transféré à {NewLeader} par {UserId}",
                code, newLeaderUserId, userId);

            return Ok(new { Message = "Leadership transféré." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Dissout un convoi (leader uniquement).
    /// </summary>
    [HttpDelete("{code}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DissolveConvoy(string code)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new DissolveConvoyCommand
            {
                JoinCode = code.ToUpperInvariant(),
                RequestingUserId = userId
            };

            await _mediator.Send(command);

            _logger.LogInformation("Convoi {Code} dissous par {UserId}", code, userId);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (DomainException ex)
        {
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
            throw new UnauthorizedAccessException("Utilisateur non authentifié ou ID invalide.");

        return userId;
    }
}
