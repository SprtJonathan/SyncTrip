using MediatR;
using SyncTrip.Shared.DTOs.Vehicles;

namespace SyncTrip.Application.Vehicles.Queries;

/// <summary>
/// Query pour récupérer tous les véhicules d'un utilisateur.
/// </summary>
public record GetUserVehiclesQuery : IRequest<IList<VehicleDto>>
{
    /// <summary>
    /// Identifiant de l'utilisateur.
    /// </summary>
    public Guid UserId { get; init; }

    public GetUserVehiclesQuery(Guid userId)
    {
        UserId = userId;
    }
}
