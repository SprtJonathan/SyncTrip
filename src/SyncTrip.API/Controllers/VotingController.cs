using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SyncTrip.Application.Voting.Commands;
using SyncTrip.Application.Voting.Queries;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;
using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.API.Controllers;

/// <summary>
/// Contrôleur pour le système de vote sur les propositions d'arrêt.
/// </summary>
[ApiController]
[Route("api/convoys/{convoyId:guid}/trips/{tripId:guid}/proposals")]
[Produces("application/json")]
[Authorize]
public class VotingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<VotingController> _logger;

    public VotingController(IMediator mediator, ILogger<VotingController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Propose un arrêt soumis au vote des membres.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<object>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProposeStop(Guid convoyId, Guid tripId, [FromBody] ProposeStopRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new ProposeStopCommand
            {
                TripId = tripId,
                UserId = userId,
                StopType = (StopType)request.StopType,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                LocationName = request.LocationName
            };

            var proposalId = await _mediator.Send(command);

            _logger.LogInformation("Proposition d'arrêt {ProposalId} créée pour le voyage {TripId} par {UserId}",
                proposalId, tripId, userId);

            return Created(string.Empty, new { ProposalId = proposalId });
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
        catch (DomainException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Récupère la proposition active du voyage.
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType<StopProposalDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActiveProposal(Guid convoyId, Guid tripId)
    {
        var query = new GetActiveProposalQuery(tripId);
        var proposal = await _mediator.Send(query);

        if (proposal == null)
            return NotFound(new { Message = "Aucune proposition active pour ce voyage." });

        return Ok(proposal);
    }

    /// <summary>
    /// Récupère l'historique des propositions du voyage.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IList<StopProposalDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProposalHistory(Guid convoyId, Guid tripId)
    {
        var query = new GetProposalHistoryQuery(tripId);
        var proposals = await _mediator.Send(query);
        return Ok(proposals);
    }

    /// <summary>
    /// Vote sur une proposition d'arrêt.
    /// </summary>
    [HttpPost("{proposalId:guid}/vote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CastVote(Guid convoyId, Guid tripId, Guid proposalId, [FromBody] CastVoteRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new CastVoteCommand
            {
                ProposalId = proposalId,
                UserId = userId,
                IsYes = request.IsYes
            };

            await _mediator.Send(command);

            _logger.LogInformation("Vote enregistré sur la proposition {ProposalId} par {UserId}",
                proposalId, userId);

            return Ok(new { Message = "Vote enregistré." });
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
