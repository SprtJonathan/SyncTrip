using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyncTrip.Api.Application.DTOs.Common;
using SyncTrip.Api.Application.DTOs.Convoys;
using SyncTrip.Api.Core.Interfaces;

namespace SyncTrip.Api.API.Controllers;

/// <summary>
/// Controller de gestion des convois
/// </summary>
[ApiController]
[Route("api/convoys")]
[Authorize]
public class ConvoysController : ControllerBase
{
    private readonly IConvoyService _convoyService;
    private readonly ILogger<ConvoysController> _logger;

    public ConvoysController(IConvoyService convoyService, ILogger<ConvoysController> logger)
    {
        _convoyService = convoyService;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;
    }

    /// <summary>
    /// Crée un nouveau convoi
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ConvoyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ConvoyDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateConvoy([FromBody] CreateConvoyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var convoy = await _convoyService.CreateConvoyAsync(userId, request, cancellationToken);
            return Ok(ApiResponse<ConvoyDto>.SuccessResult(convoy, "Convoi créé avec succès"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du convoi");
            return BadRequest(ApiResponse<ConvoyDto>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Rejoint un convoi existant avec son code
    /// </summary>
    [HttpPost("join")]
    [ProducesResponseType(typeof(ApiResponse<ConvoyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ConvoyDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> JoinConvoy([FromBody] JoinConvoyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var convoy = await _convoyService.JoinConvoyAsync(userId, request, cancellationToken);
            return Ok(ApiResponse<ConvoyDto>.SuccessResult(convoy, "Vous avez rejoint le convoi"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la jointure au convoi");
            return BadRequest(ApiResponse<ConvoyDto>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Quitte un convoi
    /// </summary>
    [HttpPost("{id}/leave")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LeaveConvoy(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _convoyService.LeaveConvoyAsync(userId, id, cancellationToken);
            return Ok(ApiResponse<string>.SuccessResult("OK", "Vous avez quitté le convoi"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du départ du convoi");
            return BadRequest(ApiResponse<string>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Récupère un convoi par son ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ConvoyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ConvoyDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConvoyById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var convoy = await _convoyService.GetConvoyByIdAsync(id, cancellationToken);
            if (convoy == null)
            {
                return NotFound(ApiResponse<ConvoyDto>.FailureResult("Convoi non trouvé"));
            }
            return Ok(ApiResponse<ConvoyDto>.SuccessResult(convoy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du convoi");
            return BadRequest(ApiResponse<ConvoyDto>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Récupère un convoi par son code
    /// </summary>
    [HttpGet("by-code/{code}")]
    [ProducesResponseType(typeof(ApiResponse<ConvoyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ConvoyDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConvoyByCode(string code, CancellationToken cancellationToken)
    {
        try
        {
            var convoy = await _convoyService.GetConvoyByCodeAsync(code, cancellationToken);
            if (convoy == null)
            {
                return NotFound(ApiResponse<ConvoyDto>.FailureResult("Convoi non trouvé"));
            }
            return Ok(ApiResponse<ConvoyDto>.SuccessResult(convoy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du convoi");
            return BadRequest(ApiResponse<ConvoyDto>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Récupère tous les convois de l'utilisateur connecté
    /// </summary>
    [HttpGet("my-convoys")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ConvoyDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyConvoys(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var convoys = await _convoyService.GetUserConvoysAsync(userId, cancellationToken);
            return Ok(ApiResponse<IEnumerable<ConvoyDto>>.SuccessResult(convoys));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des convois");
            return BadRequest(ApiResponse<IEnumerable<ConvoyDto>>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Met à jour un convoi
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ConvoyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ConvoyDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateConvoy(Guid id, [FromBody] UpdateConvoyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var convoy = await _convoyService.UpdateConvoyAsync(id, request, cancellationToken);
            return Ok(ApiResponse<ConvoyDto>.SuccessResult(convoy, "Convoi mis à jour"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour du convoi");
            return BadRequest(ApiResponse<ConvoyDto>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Supprime un convoi
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteConvoy(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _convoyService.DeleteConvoyAsync(id, cancellationToken);
            return Ok(ApiResponse<string>.SuccessResult("OK", "Convoi supprimé"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression du convoi");
            return BadRequest(ApiResponse<string>.FailureResult(ex.Message));
        }
    }
}
