using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Chat;

namespace SyncTrip.Application.Chat.Queries;

/// <summary>
/// Handler pour la query de récupération des messages d'un convoi.
/// </summary>
public class GetConvoyMessagesQueryHandler : IRequestHandler<GetConvoyMessagesQuery, IList<MessageDto>>
{
    private readonly IConvoyRepository _convoyRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<GetConvoyMessagesQueryHandler> _logger;

    public GetConvoyMessagesQueryHandler(
        IConvoyRepository convoyRepository,
        IMessageRepository messageRepository,
        ILogger<GetConvoyMessagesQueryHandler> logger)
    {
        _convoyRepository = convoyRepository;
        _messageRepository = messageRepository;
        _logger = logger;
    }

    public async Task<IList<MessageDto>> Handle(GetConvoyMessagesQuery request, CancellationToken cancellationToken)
    {
        // Récupérer le convoi avec ses membres
        var convoy = await _convoyRepository.GetByIdAsync(request.ConvoyId, cancellationToken);
        if (convoy == null)
            throw new KeyNotFoundException($"Convoi avec l'ID {request.ConvoyId} introuvable.");

        // Vérifier que l'utilisateur est membre du convoi
        if (!convoy.IsMember(request.UserId))
            throw new UnauthorizedAccessException("Vous n'êtes pas membre de ce convoi.");

        // Récupérer les messages paginés (inclut Sender)
        var messages = await _messageRepository.GetByConvoyIdAsync(
            request.ConvoyId, request.PageSize, request.Before, cancellationToken);

        _logger.LogInformation("Récupération de {Count} messages du convoi {ConvoyId}",
            messages.Count, request.ConvoyId);

        // Mapper vers DTOs
        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            ConvoyId = m.ConvoyId,
            SenderId = m.SenderId,
            SenderUsername = m.Sender?.Username ?? string.Empty,
            SenderAvatarUrl = m.Sender?.AvatarUrl,
            Content = m.Content,
            SentAt = m.SentAt
        }).ToList();
    }
}
