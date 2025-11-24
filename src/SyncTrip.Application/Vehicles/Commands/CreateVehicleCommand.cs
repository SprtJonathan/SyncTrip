using MediatR;
using SyncTrip.Core.Enums;

namespace SyncTrip.Application.Vehicles.Commands;

/// <summary>
/// Command pour créer un nouveau véhicule pour un utilisateur.
/// </summary>
public record CreateVehicleCommand : IRequest<Guid>
{
    /// <summary>
    /// Identifiant de l'utilisateur propriétaire.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Identifiant de la marque.
    /// </summary>
    public int BrandId { get; init; }

    /// <summary>
    /// Modèle du véhicule.
    /// </summary>
    public string Model { get; init; } = string.Empty;

    /// <summary>
    /// Type de véhicule.
    /// </summary>
    public VehicleType Type { get; init; }

    /// <summary>
    /// Couleur du véhicule (facultatif).
    /// </summary>
    public string? Color { get; init; }

    /// <summary>
    /// Année de fabrication (facultatif).
    /// </summary>
    public int? Year { get; init; }
}
