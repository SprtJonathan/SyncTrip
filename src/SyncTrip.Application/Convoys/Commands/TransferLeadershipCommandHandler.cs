using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Convoys.Commands;

/// <summary>
/// Handler pour transférer le leadership d'un convoi.
/// </summary>
public class TransferLeadershipCommandHandler : IRequestHandler<TransferLeadershipCommand>
{
    private readonly IConvoyRepository _convoyRepository;
    private readonly ILogger<TransferLeadershipCommandHandler> _logger;

    public TransferLeadershipCommandHandler(
        IConvoyRepository convoyRepository,
        ILogger<TransferLeadershipCommandHandler> logger)
    {
        _convoyRepository = convoyRepository;
        _logger = logger;
    }

    public async Task Handle(TransferLeadershipCommand request, CancellationToken cancellationToken)
    {
        var convoy = await _convoyRepository.GetByJoinCodeAsync(request.JoinCode, cancellationToken);
        if (convoy == null)
            throw new KeyNotFoundException($"Convoi avec le code '{request.JoinCode}' introuvable.");

        // La validation des permissions est dans l'entité
        convoy.TransferLeadership(request.RequestingUserId, request.NewLeaderUserId);

        await _convoyRepository.UpdateAsync(convoy, cancellationToken);

        _logger.LogInformation("Leadership du convoi {ConvoyId} transféré de {OldLeader} à {NewLeader}",
            convoy.Id, request.RequestingUserId, request.NewLeaderUserId);
    }
}
