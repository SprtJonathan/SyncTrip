using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SyncTrip.Application.Navigation.Commands;
using SyncTrip.Application.Navigation.Queries;
using SyncTrip.Core.Enums;
using SyncTrip.Shared.DTOs.Navigation;

namespace SyncTrip.API.Controllers;

[ApiController]
[Route("api/navigation")]
[Produces("application/json")]
[Authorize]
public class NavigationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NavigationController> _logger;

    public NavigationController(IMediator mediator, ILogger<NavigationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchAddress([FromQuery] string query, [FromQuery] int limit = 5)
    {
        try
        {
            var result = await _mediator.Send(new SearchAddressQuery
            {
                Query = query,
                Limit = limit
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la recherche d'adresse : {Query}", query);
            return BadRequest(new { Message = "Erreur lors de la recherche d'adresse." });
        }
    }

    [HttpPost("route")]
    public async Task<IActionResult> CalculateRoute([FromBody] CalculateRouteRequest request)
    {
        try
        {
            var result = await _mediator.Send(new CalculateRouteQuery
            {
                RouteProfile = (RouteProfile)request.RouteProfile,
                Waypoints = request.Waypoints
            });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("trips/{tripId:guid}/route")]
    public async Task<IActionResult> CalculateTripRoute(Guid tripId)
    {
        var userId = GetCurrentUserId();

        try
        {
            var result = await _mediator.Send(new CalculateTripRouteCommand
            {
                TripId = tripId,
                UserId = userId
            });
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Utilisateur non authentifie.");
        return userId;
    }
}
