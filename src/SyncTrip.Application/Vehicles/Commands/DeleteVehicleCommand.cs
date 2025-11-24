using MediatR;

namespace SyncTrip.Application.Vehicles.Commands;

/// <summary>
/// Command pour supprimer un véhicule.
/// </summary>
public record DeleteVehicleCommand : IRequest
{
    /// <summary>
    /// Identifiant du véhicule à supprimer.
    /// </summary>
    public Guid VehicleId { get; init; }

    /// <summary>
    /// Identifiant de l'utilisateur propriétaire (pour vérification des droits).
    /// </summary>
    public Guid UserId { get; init; }

    public DeleteVehicleCommand(Guid vehicleId, Guid userId)
    {
        VehicleId = vehicleId;
        UserId = userId;
    }
}
