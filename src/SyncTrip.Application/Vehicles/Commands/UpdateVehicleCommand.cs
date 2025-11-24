using MediatR;

namespace SyncTrip.Application.Vehicles.Commands;

/// <summary>
/// Command pour mettre à jour un véhicule existant.
/// </summary>
public record UpdateVehicleCommand : IRequest
{
    /// <summary>
    /// Identifiant du véhicule à mettre à jour.
    /// </summary>
    public Guid VehicleId { get; init; }

    /// <summary>
    /// Identifiant de l'utilisateur propriétaire (pour vérification des droits).
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Nouveau modèle du véhicule.
    /// </summary>
    public string? Model { get; init; }

    /// <summary>
    /// Nouvelle couleur du véhicule.
    /// </summary>
    public string? Color { get; init; }

    /// <summary>
    /// Nouvelle année de fabrication.
    /// </summary>
    public int? Year { get; init; }
}
