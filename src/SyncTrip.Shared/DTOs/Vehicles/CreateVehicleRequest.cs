namespace SyncTrip.Shared.DTOs.Vehicles;

/// <summary>
/// Requête pour créer un nouveau véhicule.
/// </summary>
public record CreateVehicleRequest
{
    /// <summary>
    /// Identifiant de la marque.
    /// </summary>
    public int BrandId { get; init; }

    /// <summary>
    /// Modèle du véhicule.
    /// </summary>
    public string Model { get; init; } = string.Empty;

    /// <summary>
    /// Type de véhicule (enum VehicleType : 1=Car, 2=Motorcycle, 3=Truck, 4=Van, 5=Motorhome).
    /// </summary>
    public int Type { get; init; }

    /// <summary>
    /// Couleur du véhicule (facultatif).
    /// </summary>
    public string? Color { get; init; }

    /// <summary>
    /// Année de fabrication (facultatif).
    /// </summary>
    public int? Year { get; init; }
}
