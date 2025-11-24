using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Vehicles;

namespace SyncTrip.Application.Vehicles.Queries;

/// <summary>
/// Handler pour récupérer tous les véhicules d'un utilisateur.
/// </summary>
public class GetUserVehiclesQueryHandler : IRequestHandler<GetUserVehiclesQuery, IList<VehicleDto>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<GetUserVehiclesQueryHandler> _logger;

    public GetUserVehiclesQueryHandler(
        IVehicleRepository vehicleRepository,
        ILogger<GetUserVehiclesQueryHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    /// <summary>
    /// Récupère tous les véhicules d'un utilisateur avec les informations de marque.
    /// </summary>
    /// <param name="request">Query contenant l'ID utilisateur.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des véhicules de l'utilisateur.</returns>
    public async Task<IList<VehicleDto>> Handle(GetUserVehiclesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Récupération des véhicules de l'utilisateur {UserId}", request.UserId);

        var vehicles = await _vehicleRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return vehicles.Select(v => new VehicleDto
        {
            Id = v.Id,
            BrandId = v.BrandId,
            BrandName = v.Brand.Name,
            BrandLogoUrl = v.Brand.LogoUrl,
            Model = v.Model,
            Type = (int)v.Type,
            Color = v.Color,
            Year = v.Year,
            CreatedAt = v.CreatedAt
        }).ToList();
    }
}
