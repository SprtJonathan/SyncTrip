using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Vehicles.Commands;

/// <summary>
/// Handler pour mettre à jour un véhicule existant.
/// </summary>
public class UpdateVehicleCommandHandler : IRequestHandler<UpdateVehicleCommand>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<UpdateVehicleCommandHandler> _logger;

    public UpdateVehicleCommandHandler(
        IVehicleRepository vehicleRepository,
        ILogger<UpdateVehicleCommandHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    /// <summary>
    /// Met à jour les informations d'un véhicule.
    /// </summary>
    /// <param name="request">Command contenant les nouvelles informations.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <exception cref="KeyNotFoundException">Si le véhicule n'existe pas.</exception>
    /// <exception cref="UnauthorizedAccessException">Si l'utilisateur n'est pas le propriétaire.</exception>
    public async Task Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId, cancellationToken);

        if (vehicle == null)
        {
            _logger.LogWarning("Véhicule introuvable lors de la mise à jour : {VehicleId}", request.VehicleId);
            throw new KeyNotFoundException($"Véhicule avec l'ID {request.VehicleId} introuvable");
        }

        // Vérifier que l'utilisateur est le propriétaire du véhicule
        if (vehicle.UserId != request.UserId)
        {
            _logger.LogWarning(
                "Tentative de modification non autorisée du véhicule {VehicleId} par l'utilisateur {UserId}",
                request.VehicleId,
                request.UserId
            );
            throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à modifier ce véhicule");
        }

        // Mettre à jour le véhicule
        vehicle.Update(request.Model, request.Color, request.Year);

        await _vehicleRepository.UpdateAsync(vehicle, cancellationToken);

        _logger.LogInformation("Véhicule mis à jour : {VehicleId}", vehicle.Id);
    }
}
