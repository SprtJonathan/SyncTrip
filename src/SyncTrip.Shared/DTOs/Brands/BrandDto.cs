namespace SyncTrip.Shared.DTOs.Brands;

/// <summary>
/// DTO représentant une marque de véhicule.
/// </summary>
public class BrandDto
{
    /// <summary>
    /// Identifiant unique de la marque.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Nom de la marque (ex: "Yamaha", "Renault", "BMW").
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// URL du logo de la marque.
    /// </summary>
    public string LogoUrl { get; init; } = string.Empty;
}
