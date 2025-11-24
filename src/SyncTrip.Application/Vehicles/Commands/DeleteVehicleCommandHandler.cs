using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Vehicles.Commands;

/// <summary>
/// Handler pour supprimer un véhicule.
/// </summary>
public class DeleteVehicleCommandHandler : IRequestHandler<DeleteVehicleCommand>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<DeleteVehicleCommandHandler> _logger;

    public DeleteVehicleCommandHandler(
        IVehicleRepository vehicleRepository,
        ILogger<DeleteVehicleCommandHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    /// <summary>
    /// Supprime un véhicule après vérification des droits.
    /// </summary>
    /// <param name="request">Command contenant l'ID du véhicule.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <exception cref="KeyNotFoundException">Si le véhicule n'existe pas.</exception>
    /// <exception cref="UnauthorizedAccessException">Si l'utilisateur n'est pas le propriétaire.</exception>
    public async Task Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId, cancellationToken);

        if (vehicle == null)
        {
            _logger.LogWarning("Véhicule introuvable lors de la suppression : {VehicleId}", request.VehicleId);
            throw new KeyNotFoundException($"Véhicule avec l'ID {request.VehicleId} introuvable");
        }

        // Vérifier que l'utilisateur est le propriétaire du véhicule
        if (vehicle.UserId != request.UserId)
        {
            _logger.LogWarning(
                "Tentative de suppression non autorisée du véhicule {VehicleId} par l'utilisateur {UserId}",
                request.VehicleId,
                request.UserId
            );
            throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à supprimer ce véhicule");
        }

        await _vehicleRepository.DeleteAsync(vehicle, cancellationToken);

        _logger.LogInformation("Véhicule supprimé : {VehicleId}", request.VehicleId);
    }
}
