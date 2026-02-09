using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Convoys.Commands;

/// <summary>
/// Handler pour créer un nouveau convoi.
/// </summary>
public class CreateConvoyCommandHandler : IRequestHandler<CreateConvoyCommand, Guid>
{
    private readonly IConvoyRepository _convoyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<CreateConvoyCommandHandler> _logger;

    public CreateConvoyCommandHandler(
        IConvoyRepository convoyRepository,
        IUserRepository userRepository,
        IVehicleRepository vehicleRepository,
        ILogger<CreateConvoyCommandHandler> logger)
    {
        _convoyRepository = convoyRepository;
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateConvoyCommand request, CancellationToken cancellationToken)
    {
        // Vérifier que l'utilisateur existe
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException($"Utilisateur avec l'ID {request.UserId} introuvable.");

        // Vérifier que le véhicule existe et appartient à l'utilisateur
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId, cancellationToken);
        if (vehicle == null)
            throw new KeyNotFoundException($"Véhicule avec l'ID {request.VehicleId} introuvable.");

        if (vehicle.UserId != request.UserId)
            throw new UnauthorizedAccessException("Ce véhicule ne vous appartient pas.");

        // Créer le convoi (le leader est ajouté automatiquement comme membre)
        var convoy = Convoy.Create(request.UserId, request.VehicleId, request.IsPrivate);

        await _convoyRepository.AddAsync(convoy, cancellationToken);

        _logger.LogInformation("Nouveau convoi créé : {ConvoyId} (code: {JoinCode}) par {UserId}",
            convoy.Id, convoy.JoinCode, request.UserId);

        return convoy.Id;
    }
}
