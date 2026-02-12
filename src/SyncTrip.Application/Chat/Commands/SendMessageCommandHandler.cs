using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Application.Chat.Services;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Chat;

namespace SyncTrip.Application.Chat.Commands;

/// <summary>
/// Handler pour la commande d'envoi de message.
/// </summary>
public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Guid>
{
    private readonly IConvoyRepository _convoyRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IConvoyNotificationService _notificationService;
    private readonly ILogger<SendMessageCommandHandler> _logger;

    public SendMessageCommandHandler(
        IConvoyRepository convoyRepository,
        IMessageRepository messageRepository,
        IUserRepository userRepository,
        IConvoyNotificationService notificationService,
        ILogger<SendMessageCommandHandler> logger)
    {
        _convoyRepository = convoyRepository;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Guid> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        // Récupérer le convoi avec ses membres
        var convoy = await _convoyRepository.GetByIdAsync(request.ConvoyId, cancellationToken);
        if (convoy == null)
            throw new KeyNotFoundException($"Convoi avec l'ID {request.ConvoyId} introuvable.");

        // Vérifier que l'utilisateur est membre du convoi
        if (!convoy.IsMember(request.UserId))
            throw new UnauthorizedAccessException("Vous n'êtes pas membre de ce convoi.");

        // Créer le message
        var message = Core.Entities.Message.Create(request.ConvoyId, request.UserId, request.Content);

        // Persister le message
        await _messageRepository.AddAsync(message, cancellationToken);

        // Charger le sender pour le DTO
        var sender = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        // Notifier les membres via SignalR
        var messageDto = new MessageDto
        {
            Id = message.Id,
            ConvoyId = message.ConvoyId,
            SenderId = message.SenderId,
            SenderUsername = sender?.Username ?? string.Empty,
            SenderAvatarUrl = sender?.AvatarUrl,
            Content = message.Content,
            SentAt = message.SentAt
        };

        await _notificationService.NotifyNewMessageAsync(request.ConvoyId, messageDto, cancellationToken);

        _logger.LogInformation("Message {MessageId} envoyé dans le convoi {ConvoyId} par {UserId}",
            message.Id, request.ConvoyId, request.UserId);

        return message.Id;
    }
}
