using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Convoys.Commands;

/// <summary>
/// Handler pour quitter un convoi.
/// </summary>
public class LeaveConvoyCommandHandler : IRequestHandler<LeaveConvoyCommand>
{
    private readonly IConvoyRepository _convoyRepository;
    private readonly ILogger<LeaveConvoyCommandHandler> _logger;

    public LeaveConvoyCommandHandler(
        IConvoyRepository convoyRepository,
        ILogger<LeaveConvoyCommandHandler> logger)
    {
        _convoyRepository = convoyRepository;
        _logger = logger;
    }

    public async Task Handle(LeaveConvoyCommand request, CancellationToken cancellationToken)
    {
        var convoy = await _convoyRepository.GetByJoinCodeAsync(request.JoinCode, cancellationToken);
        if (convoy == null)
            throw new KeyNotFoundException($"Convoi avec le code '{request.JoinCode}' introuvable.");

        // La validation (leader ne peut pas quitter) est dans l'entité
        convoy.RemoveMember(request.UserId);

        await _convoyRepository.UpdateAsync(convoy, cancellationToken);

        _logger.LogInformation("Utilisateur {UserId} a quitté le convoi {ConvoyId}", request.UserId, convoy.Id);
    }
}
