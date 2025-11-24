namespace SyncTrip.Shared.DTOs.Vehicles;

/// <summary>
/// DTO représentant un véhicule.
/// </summary>
public class VehicleDto
{
    /// <summary>
    /// Identifiant unique du véhicule.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifiant de la marque.
    /// </summary>
    public int BrandId { get; init; }

    /// <summary>
    /// Nom de la marque.
    /// </summary>
    public string BrandName { get; init; } = string.Empty;

    /// <summary>
    /// URL du logo de la marque.
    /// </summary>
    public string BrandLogoUrl { get; init; } = string.Empty;

    /// <summary>
    /// Modèle du véhicule.
    /// </summary>
    public string Model { get; init; } = string.Empty;

    /// <summary>
    /// Type de véhicule (enum VehicleType).
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

    /// <summary>
    /// Date d'ajout du véhicule.
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
