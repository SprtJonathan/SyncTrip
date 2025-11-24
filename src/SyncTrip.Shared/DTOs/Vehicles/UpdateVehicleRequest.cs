namespace SyncTrip.Shared.DTOs.Vehicles;

/// <summary>
/// Requête pour mettre à jour un véhicule existant.
/// </summary>
public record UpdateVehicleRequest
{
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
