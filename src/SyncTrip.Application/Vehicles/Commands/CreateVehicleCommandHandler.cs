using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Vehicles.Commands;

/// <summary>
/// Handler pour créer un nouveau véhicule.
/// </summary>
public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, Guid>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateVehicleCommandHandler> _logger;

    public CreateVehicleCommandHandler(
        IVehicleRepository vehicleRepository,
        IBrandRepository brandRepository,
        IUserRepository userRepository,
        ILogger<CreateVehicleCommandHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _brandRepository = brandRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Crée un nouveau véhicule après validation.
    /// </summary>
    /// <param name="request">Command contenant les informations du véhicule.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Identifiant du véhicule créé.</returns>
    /// <exception cref="KeyNotFoundException">Si l'utilisateur ou la marque n'existe pas.</exception>
    public async Task<Guid> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        // Vérifier que l'utilisateur existe
        var userExists = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (userExists == null)
        {
            _logger.LogWarning("Tentative de création de véhicule pour un utilisateur inexistant : {UserId}", request.UserId);
            throw new KeyNotFoundException($"Utilisateur avec l'ID {request.UserId} introuvable");
        }

        // Vérifier que la marque existe
        var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);
        if (brand == null)
        {
            _logger.LogWarning("Tentative de création de véhicule avec une marque inexistante : {BrandId}", request.BrandId);
            throw new KeyNotFoundException($"Marque avec l'ID {request.BrandId} introuvable");
        }

        // Créer le véhicule
        var vehicle = Vehicle.Create(
            request.UserId,
            request.BrandId,
            request.Model,
            request.Type,
            request.Color,
            request.Year
        );

        await _vehicleRepository.AddAsync(vehicle, cancellationToken);

        _logger.LogInformation("Nouveau véhicule créé : {VehicleId} pour utilisateur {UserId}", vehicle.Id, request.UserId);

        return vehicle.Id;
    }
}
