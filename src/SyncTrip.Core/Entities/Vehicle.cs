using SyncTrip.Core.Enums;

namespace SyncTrip.Core.Entities;

/// <summary>
/// Représente un véhicule appartenant à un utilisateur.
/// </summary>
public class Vehicle
{
    /// <summary>
    /// Identifiant unique du véhicule.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identifiant de l'utilisateur propriétaire.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Utilisateur propriétaire du véhicule.
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Identifiant de la marque du véhicule.
    /// </summary>
    public int BrandId { get; private set; }

    /// <summary>
    /// Marque du véhicule.
    /// </summary>
    public Brand Brand { get; private set; } = null!;

    /// <summary>
    /// Modèle du véhicule (ex: "Clio", "CBR 600", "Ducato").
    /// </summary>
    public string Model { get; private set; } = string.Empty;

    /// <summary>
    /// Type de véhicule (voiture, moto, camion, etc.).
    /// </summary>
    public VehicleType Type { get; private set; }

    /// <summary>
    /// Couleur du véhicule (facultatif).
    /// </summary>
    public string? Color { get; private set; }

    /// <summary>
    /// Année de fabrication (facultatif).
    /// </summary>
    public int? Year { get; private set; }

    /// <summary>
    /// Date de création de l'enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    // Constructeur privé pour EF Core
    private Vehicle()
    {
    }

    /// <summary>
    /// Factory method pour créer un nouveau véhicule.
    /// </summary>
    /// <param name="userId">Identifiant du propriétaire.</param>
    /// <param name="brandId">Identifiant de la marque.</param>
    /// <param name="model">Modèle du véhicule.</param>
    /// <param name="type">Type de véhicule.</param>
    /// <param name="color">Couleur (facultatif).</param>
    /// <param name="year">Année (facultatif).</param>
    /// <returns>Nouvelle instance de Vehicle.</returns>
    /// <exception cref="ArgumentException">Si les données sont invalides.</exception>
    public static Vehicle Create(
        Guid userId,
        int brandId,
        string model,
        VehicleType type,
        string? color = null,
        int? year = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("L'identifiant utilisateur est invalide.", nameof(userId));

        if (brandId <= 0)
            throw new ArgumentException("L'identifiant de la marque est invalide.", nameof(brandId));

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Le modèle du véhicule est obligatoire.", nameof(model));

        if (year.HasValue && (year.Value < 1900 || year.Value > DateTime.UtcNow.Year + 1))
            throw new ArgumentException($"L'année doit être comprise entre 1900 et {DateTime.UtcNow.Year + 1}.", nameof(year));

        return new Vehicle
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BrandId = brandId,
            Model = model.Trim(),
            Type = type,
            Color = string.IsNullOrWhiteSpace(color) ? null : color.Trim(),
            Year = year,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Met à jour les informations du véhicule.
    /// </summary>
    /// <param name="model">Nouveau modèle.</param>
    /// <param name="color">Nouvelle couleur.</param>
    /// <param name="year">Nouvelle année.</param>
    public void Update(string? model = null, string? color = null, int? year = null)
    {
        if (!string.IsNullOrWhiteSpace(model))
            Model = model.Trim();

        Color = string.IsNullOrWhiteSpace(color) ? null : color.Trim();

        if (year.HasValue)
        {
            if (year.Value < 1900 || year.Value > DateTime.UtcNow.Year + 1)
                throw new ArgumentException($"L'année doit être comprise entre 1900 et {DateTime.UtcNow.Year + 1}.", nameof(year));

            Year = year;
        }
    }
}
