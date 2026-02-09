using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Convoys.Commands;

/// <summary>
/// Handler pour exclure un membre du convoi.
/// </summary>
public class KickMemberCommandHandler : IRequestHandler<KickMemberCommand>
{
    private readonly IConvoyRepository _convoyRepository;
    private readonly ILogger<KickMemberCommandHandler> _logger;

    public KickMemberCommandHandler(
        IConvoyRepository convoyRepository,
        ILogger<KickMemberCommandHandler> logger)
    {
        _convoyRepository = convoyRepository;
        _logger = logger;
    }

    public async Task Handle(KickMemberCommand request, CancellationToken cancellationToken)
    {
        var convoy = await _convoyRepository.GetByJoinCodeAsync(request.JoinCode, cancellationToken);
        if (convoy == null)
            throw new KeyNotFoundException($"Convoi avec le code '{request.JoinCode}' introuvable.");

        // La validation des permissions leader est dans l'entité
        convoy.KickMember(request.RequestingUserId, request.TargetUserId);

        await _convoyRepository.UpdateAsync(convoy, cancellationToken);

        _logger.LogInformation("Utilisateur {TargetUserId} exclu du convoi {ConvoyId} par {RequestingUserId}",
            request.TargetUserId, convoy.Id, request.RequestingUserId);
    }
}
