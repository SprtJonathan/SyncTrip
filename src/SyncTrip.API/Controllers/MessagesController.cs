using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SyncTrip.Application.Chat.Commands;
using SyncTrip.Application.Chat.Queries;
using SyncTrip.Core.Exceptions;
using SyncTrip.Shared.DTOs.Chat;

namespace SyncTrip.API.Controllers;

/// <summary>
/// Contrôleur pour le chat des convois.
/// </summary>
[ApiController]
[Route("api/convoys/{convoyId:guid}/messages")]
[Produces("application/json")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(IMediator mediator, ILogger<MessagesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Envoie un message dans le chat du convoi.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<object>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendMessage(Guid convoyId, [FromBody] SendMessageRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var command = new SendMessageCommand
            {
                ConvoyId = convoyId,
                UserId = userId,
                Content = request.Content
            };

            var messageId = await _mediator.Send(command);

            _logger.LogInformation("Message {MessageId} envoyé dans le convoi {ConvoyId} par {UserId}",
                messageId, convoyId, userId);

            return Created(string.Empty, new { MessageId = messageId });
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
    /// Récupère l'historique des messages du convoi avec pagination par curseur.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IList<MessageDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessages(Guid convoyId, [FromQuery] int pageSize = 50, [FromQuery] DateTime? before = null)
    {
        var userId = GetCurrentUserId();

        try
        {
            var query = new GetConvoyMessagesQuery
            {
                ConvoyId = convoyId,
                UserId = userId,
                PageSize = pageSize,
                Before = before
            };

            var messages = await _mediator.Send(query);
            return Ok(messages);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
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
            throw new UnauthorizedAccessException("Utilisateur non authentifié ou ID invalide.");

        return userId;
    }
}
