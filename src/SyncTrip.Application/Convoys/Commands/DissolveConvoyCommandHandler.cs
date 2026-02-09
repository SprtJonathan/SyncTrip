using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Convoys.Commands;

/// <summary>
/// Handler pour dissoudre un convoi.
/// </summary>
public class DissolveConvoyCommandHandler : IRequestHandler<DissolveConvoyCommand>
{
    private readonly IConvoyRepository _convoyRepository;
    private readonly ILogger<DissolveConvoyCommandHandler> _logger;

    public DissolveConvoyCommandHandler(
        IConvoyRepository convoyRepository,
        ILogger<DissolveConvoyCommandHandler> logger)
    {
        _convoyRepository = convoyRepository;
        _logger = logger;
    }

    public async Task Handle(DissolveConvoyCommand request, CancellationToken cancellationToken)
    {
        var convoy = await _convoyRepository.GetByJoinCodeAsync(request.JoinCode, cancellationToken);
        if (convoy == null)
            throw new KeyNotFoundException($"Convoi avec le code '{request.JoinCode}' introuvable.");

        // Vérifier que c'est bien le leader
        convoy.EnsureIsLeader(request.RequestingUserId);

        await _convoyRepository.DeleteAsync(convoy, cancellationToken);

        _logger.LogInformation("Convoi {ConvoyId} (code: {JoinCode}) dissous par {UserId}",
            convoy.Id, request.JoinCode, request.RequestingUserId);
    }
}
