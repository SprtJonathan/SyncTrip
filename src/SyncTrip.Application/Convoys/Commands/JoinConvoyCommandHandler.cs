using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Convoys.Commands;

/// <summary>
/// Handler pour rejoindre un convoi.
/// </summary>
public class JoinConvoyCommandHandler : IRequestHandler<JoinConvoyCommand>
{
    private readonly IConvoyRepository _convoyRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<JoinConvoyCommandHandler> _logger;

    public JoinConvoyCommandHandler(
        IConvoyRepository convoyRepository,
        IVehicleRepository vehicleRepository,
        ILogger<JoinConvoyCommandHandler> logger)
    {
        _convoyRepository = convoyRepository;
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    public async Task Handle(JoinConvoyCommand request, CancellationToken cancellationToken)
    {
        // Trouver le convoi par code
        var convoy = await _convoyRepository.GetByJoinCodeAsync(request.JoinCode, cancellationToken);
        if (convoy == null)
            throw new KeyNotFoundException($"Convoi avec le code '{request.JoinCode}' introuvable.");

        // Vérifier que le véhicule existe et appartient à l'utilisateur
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId, cancellationToken);
        if (vehicle == null)
            throw new KeyNotFoundException($"Véhicule avec l'ID {request.VehicleId} introuvable.");

        if (vehicle.UserId != request.UserId)
            throw new UnauthorizedAccessException("Ce véhicule ne vous appartient pas.");

        // Ajouter le membre (la validation de doublon est dans l'entité)
        convoy.AddMember(request.UserId, request.VehicleId);

        await _convoyRepository.UpdateAsync(convoy, cancellationToken);

        _logger.LogInformation("Utilisateur {UserId} a rejoint le convoi {ConvoyId} (code: {JoinCode})",
            request.UserId, convoy.Id, request.JoinCode);
    }
}
