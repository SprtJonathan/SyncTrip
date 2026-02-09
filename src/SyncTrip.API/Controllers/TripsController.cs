using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SyncTrip.Application.Trips.Commands;
using SyncTrip.Application.Trips.Queries;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;
using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.API.Controllers;

/// <summary>
/// Contrôleur pour la gestion des voyages GPS dans un convoi.
/// </summary>
[ApiController]
[Route("api/convoys/{convoyId:guid}/trips")]
[Produces("application/json")]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TripsController> _logger;

    public TripsController(IMediator mediator, ILogger<TripsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Démarre un nouveau voyage (leader uniquement).
    /// </summary>
    [HttpPost]
    [ProducesResponseType<object>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> StartTrip(Guid convoyId, [FromBody] StartTripRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new StartTripCommand
            {
                ConvoyId = convoyId,
                UserId = userId,
                Status = (TripStatus)request.Status,
                RouteProfile = (RouteProfile)request.RouteProfile,
                Waypoints = request.Waypoints
            };

            var tripId = await _mediator.Send(command);

            _logger.LogInformation("Voyage {TripId} démarré pour le convoi {ConvoyId} par {UserId}",
                tripId, convoyId, userId);

            return CreatedAtAction(
                nameof(GetTrip),
                new { convoyId, tripId },
                new { TripId = tripId }
            );
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Récupère le voyage actif du convoi.
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType<TripDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActiveTrip(Guid convoyId)
    {
        var query = new GetActiveTripByConvoyIdQuery(convoyId);
        var trip = await _mediator.Send(query);

        if (trip == null)
            return NotFound(new { Message = "Aucun voyage actif pour ce convoi." });

        return Ok(trip);
    }

    /// <summary>
    /// Récupère un voyage par son identifiant.
    /// </summary>
    [HttpGet("{tripId:guid}")]
    [ProducesResponseType<TripDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrip(Guid convoyId, Guid tripId)
    {
        var query = new GetTripByIdQuery(tripId);
        var trip = await _mediator.Send(query);

        if (trip == null)
            return NotFound(new { Message = "Voyage introuvable." });

        return Ok(trip);
    }

    /// <summary>
    /// Récupère l'historique des voyages d'un convoi.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IList<TripDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConvoyTrips(Guid convoyId)
    {
        var query = new GetConvoyTripsQuery(convoyId);
        var trips = await _mediator.Send(query);
        return Ok(trips);
    }

    /// <summary>
    /// Termine un voyage (leader uniquement).
    /// </summary>
    [HttpPost("{tripId:guid}/end")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EndTrip(Guid convoyId, Guid tripId)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new EndTripCommand
            {
                TripId = tripId,
                UserId = userId
            };

            await _mediator.Send(command);

            _logger.LogInformation("Voyage {TripId} terminé par {UserId}", tripId, userId);

            return Ok(new { Message = "Voyage terminé." });
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
    /// Ajoute un waypoint à un voyage (membres du convoi).
    /// </summary>
    [HttpPost("{tripId:guid}/waypoints")]
    [ProducesResponseType<object>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddWaypoint(Guid convoyId, Guid tripId, [FromBody] AddWaypointRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new AddWaypointCommand
            {
                TripId = tripId,
                UserId = userId,
                OrderIndex = request.OrderIndex,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Name = request.Name,
                Type = (WaypointType)request.Type
            };

            var waypointId = await _mediator.Send(command);

            _logger.LogInformation("Waypoint {WaypointId} ajouté au voyage {TripId} par {UserId}",
                waypointId, tripId, userId);

            return Created(string.Empty, new { WaypointId = waypointId });
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
    /// Supprime un waypoint d'un voyage (leader uniquement).
    /// </summary>
    [HttpDelete("{tripId:guid}/waypoints/{waypointId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveWaypoint(Guid convoyId, Guid tripId, Guid waypointId)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new RemoveWaypointCommand
            {
                TripId = tripId,
                WaypointId = waypointId,
                UserId = userId
            };

            await _mediator.Send(command);

            _logger.LogInformation("Waypoint {WaypointId} supprimé du voyage {TripId} par {UserId}",
                waypointId, tripId, userId);

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
