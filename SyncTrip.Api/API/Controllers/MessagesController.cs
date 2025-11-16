using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyncTrip.Api.Application.DTOs.Common;
using SyncTrip.Api.Application.DTOs.Messages;
using SyncTrip.Api.Core.Interfaces;

namespace SyncTrip.Api.API.Controllers;

/// <summary>
/// Controller de gestion des messages
/// </summary>
[ApiController]
[Route("api/convoys/{convoyId}/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(IMessageService messageService, ILogger<MessagesController> logger)
    {
        _messageService = messageService;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;
    }

    /// <summary>
    /// Envoie un message dans un convoi
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendMessage(Guid convoyId, [FromBody] SendMessageRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var message = await _messageService.SendMessageAsync(userId, convoyId, request, cancellationToken);
            return Ok(ApiResponse<MessageDto>.SuccessResult(message, "Message envoyé"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<MessageDto>.FailureResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi du message");
            return BadRequest(ApiResponse<MessageDto>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Récupère les messages d'un convoi (paginés)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MessageDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMessages(Guid convoyId, [FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken cancellationToken = default)
    {
        try
        {
            var messages = await _messageService.GetConvoyMessagesAsync(convoyId, skip, take, cancellationToken);
            return Ok(ApiResponse<IEnumerable<MessageDto>>.SuccessResult(messages));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des messages");
            return BadRequest(ApiResponse<IEnumerable<MessageDto>>.FailureResult(ex.Message));
        }
    }

    /// <summary>
    /// Supprime un message
    /// </summary>
    [HttpDelete("{messageId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteMessage(Guid convoyId, Guid messageId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _messageService.DeleteMessageAsync(messageId, userId, cancellationToken);
            return Ok(ApiResponse<string>.SuccessResult("OK", "Message supprimé"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<string>.FailureResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression du message");
            return BadRequest(ApiResponse<string>.FailureResult(ex.Message));
        }
    }
}
