using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SyncTrip.Application.Vehicles.Commands;
using SyncTrip.Application.Vehicles.Queries;
using SyncTrip.Core.Enums;
using SyncTrip.Shared.DTOs.Vehicles;

namespace SyncTrip.API.Controllers;

/// <summary>
/// Contrôleur pour la gestion des véhicules (garage utilisateur).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<VehiclesController> _logger;

    /// <summary>
    /// Initialise une nouvelle instance du contrôleur véhicules.
    /// </summary>
    public VehiclesController(IMediator mediator, ILogger<VehiclesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Récupère tous les véhicules de l'utilisateur connecté.
    /// </summary>
    /// <returns>Liste des véhicules de l'utilisateur.</returns>
    [HttpGet]
    [ProducesResponseType<IList<VehicleDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyVehicles()
    {
        var userId = GetCurrentUserId();
        var query = new GetUserVehiclesQuery(userId);
        var vehicles = await _mediator.Send(query);
        return Ok(vehicles);
    }

    /// <summary>
    /// Crée un nouveau véhicule pour l'utilisateur connecté.
    /// </summary>
    /// <param name="request">Données du véhicule à créer.</param>
    /// <returns>ID du véhicule créé.</returns>
    [HttpPost]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new CreateVehicleCommand
            {
                UserId = userId,
                BrandId = request.BrandId,
                Model = request.Model,
                Type = (VehicleType)request.Type,
                Color = request.Color,
                Year = request.Year
            };

            var vehicleId = await _mediator.Send(command);

            _logger.LogInformation("Véhicule créé : {VehicleId} par utilisateur {UserId}", vehicleId, userId);

            return CreatedAtAction(
                nameof(GetMyVehicles),
                new { id = vehicleId },
                new { VehicleId = vehicleId }
            );
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Erreur lors de la création du véhicule : {Message}", ex.Message);
            return BadRequest(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Données invalides pour la création du véhicule : {Message}", ex.Message);
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Met à jour un véhicule existant.
    /// </summary>
    /// <param name="id">ID du véhicule à mettre à jour.</param>
    /// <param name="request">Nouvelles données du véhicule.</param>
    /// <returns>Confirmation de mise à jour.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVehicle(Guid id, [FromBody] UpdateVehicleRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new UpdateVehicleCommand
            {
                VehicleId = id,
                UserId = userId,
                Model = request.Model,
                Color = request.Color,
                Year = request.Year
            };

            await _mediator.Send(command);

            _logger.LogInformation("Véhicule mis à jour : {VehicleId}", id);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Véhicule introuvable : {VehicleId}", id);
            return NotFound(new { Message = "Véhicule introuvable" });
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("Tentative de modification non autorisée du véhicule {VehicleId} par {UserId}", id, userId);
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Données invalides pour la mise à jour du véhicule : {Message}", ex.Message);
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Supprime un véhicule.
    /// </summary>
    /// <param name="id">ID du véhicule à supprimer.</param>
    /// <returns>Confirmation de suppression.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVehicle(Guid id)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new DeleteVehicleCommand(id, userId);
            await _mediator.Send(command);

            _logger.LogInformation("Véhicule supprimé : {VehicleId}", id);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Véhicule introuvable : {VehicleId}", id);
            return NotFound(new { Message = "Véhicule introuvable" });
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("Tentative de suppression non autorisée du véhicule {VehicleId} par {UserId}", id, userId);
            return Forbid();
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
