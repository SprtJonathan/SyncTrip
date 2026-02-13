using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Users.Commands;

public class DeleteUserAccountCommandHandler : IRequestHandler<DeleteUserAccountCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DeleteUserAccountCommandHandler> _logger;

    public DeleteUserAccountCommandHandler(
        IUserRepository userRepository,
        ILogger<DeleteUserAccountCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DeleteUserAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("Utilisateur introuvable lors de la suppression : {UserId}", request.UserId);
            throw new KeyNotFoundException($"Utilisateur avec l'ID {request.UserId} introuvable");
        }

        await _userRepository.DeleteAsync(request.UserId, cancellationToken);

        _logger.LogInformation("Compte utilisateur supprime : {UserId}", request.UserId);
    }
}
