using AutoMapper;
using SyncTrip.Api.Application.DTOs.Messages;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Enums;
using SyncTrip.Api.Core.Interfaces;

namespace SyncTrip.Api.Infrastructure.Services;

/// <summary>
/// Service de gestion des messages
/// </summary>
public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MessageService> _logger;

    public MessageService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<MessageService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<MessageDto> SendMessageAsync(Guid userId, Guid convoyId, SendMessageRequest request, CancellationToken cancellationToken = default)
    {
        // Vérifier que l'utilisateur est membre du convoi
        var isMember = await _unitOfWork.ConvoyParticipants.IsUserInConvoyAsync(userId, convoyId, cancellationToken);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("Vous devez être membre du convoi pour envoyer des messages");
        }

        // Créer le message
        var message = new Message
        {
            ConvoyId = convoyId,
            UserId = userId,
            Type = MessageType.User,
            Content = request.Content,
            SentAt = DateTime.UtcNow
        };

        await _unitOfWork.Messages.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Message envoyé dans le convoi {ConvoyId} par {UserId}", convoyId, userId);

        // Récupérer le message complet avec l'utilisateur
        var savedMessage = await _unitOfWork.Messages.GetByIdAsync(message.Id, cancellationToken);
        return _mapper.Map<MessageDto>(savedMessage);
    }

    public async Task<IEnumerable<MessageDto>> GetConvoyMessagesAsync(Guid convoyId, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        var messages = await _unitOfWork.Messages.GetConvoyMessagesAsync(convoyId, skip, take, cancellationToken);
        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<IEnumerable<MessageDto>> GetConvoyMessagesSinceAsync(Guid convoyId, DateTime since, CancellationToken cancellationToken = default)
    {
        var messages = await _unitOfWork.Messages.GetConvoyMessagesSinceAsync(convoyId, since, cancellationToken);
        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task DeleteMessageAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(messageId, cancellationToken);
        if (message == null)
        {
            throw new InvalidOperationException("Message non trouvé");
        }

        // Vérifier que l'utilisateur est l'auteur du message
        if (message.UserId != userId)
        {
            throw new UnauthorizedAccessException("Vous ne pouvez supprimer que vos propres messages");
        }

        message.IsDeleted = true;
        message.DeletedAt = DateTime.UtcNow;

        _unitOfWork.Messages.Update(message);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Message {MessageId} supprimé par {UserId}", messageId, userId);
    }
}
