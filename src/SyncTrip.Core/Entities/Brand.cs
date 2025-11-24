namespace SyncTrip.Core.Entities;

/// <summary>
/// Représente une marque de véhicule (constructeur).
/// Données de référence seed dans la base de données.
/// </summary>
public class Brand
{
    /// <summary>
    /// Identifiant unique de la marque.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Nom de la marque (ex: "Yamaha", "Renault", "BMW").
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// URL du logo de la marque.
    /// </summary>
    public string LogoUrl { get; private set; } = string.Empty;

    /// <summary>
    /// Collection de véhicules associés à cette marque.
    /// </summary>
    public ICollection<Vehicle> Vehicles { get; private set; } = new List<Vehicle>();

    // Constructeur privé pour EF Core
    private Brand()
    {
    }

    /// <summary>
    /// Factory method pour créer une nouvelle marque.
    /// </summary>
    /// <param name="id">Identifiant de la marque.</param>
    /// <param name="name">Nom de la marque.</param>
    /// <param name="logoUrl">URL du logo.</param>
    /// <returns>Nouvelle instance de Brand.</returns>
    public static Brand Create(int id, string name, string logoUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Le nom de la marque est obligatoire.", nameof(name));

        if (string.IsNullOrWhiteSpace(logoUrl))
            throw new ArgumentException("L'URL du logo est obligatoire.", nameof(logoUrl));

        return new Brand
        {
            Id = id,
            Name = name.Trim(),
            LogoUrl = logoUrl.Trim()
        };
    }
}
