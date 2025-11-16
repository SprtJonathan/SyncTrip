using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyncTrip.Api.Application.DTOs.Common;
using SyncTrip.Api.Application.DTOs.Trips;
using SyncTrip.Api.Application.DTOs.Waypoints;
using SyncTrip.Api.Core.Interfaces;

namespace SyncTrip.Api.API.Controllers;

/// <summary>
/// Controller de gestion des trips
/// </summary>
[ApiController]
[Route("api/trips")]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;
    private readonly ILogger<TripsController> _logger;

    public TripsController(ITripService tripService, ILogger<TripsController> logger)
    {
        _tripService = tripService;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;
    }

    /// <summary>
    /// Crée un nouveau trip
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTrip([FromBody] CreateTripRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var trip = await _tripService.CreateTripAsync(userId, request, cancellationToken);
            return Ok(ApiResponse<TripDto>.SuccessResult(trip, "Trip créé avec succès"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du trip");
            return BadRequest(ApiResponse<TripDto>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Récupère un trip par son ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTripById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var trip = await _tripService.GetTripByIdAsync(id, cancellationToken);
            if (trip == null)
            {
                return NotFound(ApiResponse<TripDto>.FailureResult("Trip non trouvé"));
            }
            return Ok(ApiResponse<TripDto>.SuccessResult(trip));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du trip");
            return BadRequest(ApiResponse<TripDto>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Récupère tous les trips d'un convoi
    /// </summary>
    [HttpGet("convoy/{convoyId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TripDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConvoyTrips(Guid convoyId, CancellationToken cancellationToken)
    {
        try
        {
            var trips = await _tripService.GetConvoyTripsAsync(convoyId, cancellationToken);
            return Ok(ApiResponse<IEnumerable<TripDto>>.SuccessResult(trips));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des trips");
            return BadRequest(ApiResponse<IEnumerable<TripDto>>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Récupère le trip actif d'un convoi
    /// </summary>
    [HttpGet("convoy/{convoyId}/active")]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveConvoyTrip(Guid convoyId, CancellationToken cancellationToken)
    {
        try
        {
            var trip = await _tripService.GetActiveConvoyTripAsync(convoyId, cancellationToken);
            return Ok(ApiResponse<TripDto?>.SuccessResult(trip));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du trip actif");
            return BadRequest(ApiResponse<TripDto>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Met à jour le statut d'un trip
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTripStatus(Guid id, [FromBody] UpdateTripStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var trip = await _tripService.UpdateTripStatusAsync(id, request, cancellationToken);
            return Ok(ApiResponse<TripDto>.SuccessResult(trip, "Statut mis à jour"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour du statut");
            return BadRequest(ApiResponse<TripDto>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Ajoute un waypoint à un trip
    /// </summary>
    [HttpPost("{id}/waypoints")]
    [ProducesResponseType(typeof(ApiResponse<WaypointDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<WaypointDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddWaypoint(Guid id, [FromBody] CreateWaypointRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var waypoint = await _tripService.AddWaypointAsync(id, request, cancellationToken);
            return Ok(ApiResponse<WaypointDto>.SuccessResult(waypoint, "Waypoint ajouté"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'ajout du waypoint");
            return BadRequest(ApiResponse<WaypointDto>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Marque un waypoint comme atteint
    /// </summary>
    [HttpPost("waypoints/{waypointId}/reached")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkWaypointReached(Guid waypointId, CancellationToken cancellationToken)
    {
        try
        {
            await _tripService.MarkWaypointReachedAsync(waypointId, cancellationToken);
            return Ok(ApiResponse<string>.SuccessResult("OK", "Waypoint marqué comme atteint"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du marquage du waypoint");
            return BadRequest(ApiResponse<string>.FailureResult(ex.Message));
        }
    }
}
