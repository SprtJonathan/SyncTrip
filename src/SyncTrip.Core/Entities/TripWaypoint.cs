using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;

namespace SyncTrip.Core.Entities;

/// <summary>
/// Représente un point de passage dans un voyage.
/// </summary>
public class TripWaypoint
{
    /// <summary>
    /// Identifiant unique du waypoint.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identifiant du voyage associé.
    /// </summary>
    public Guid TripId { get; private set; }

    /// <summary>
    /// Voyage associé.
    /// </summary>
    public Trip Trip { get; private set; } = null!;

    /// <summary>
    /// Index d'ordre dans l'itinéraire.
    /// </summary>
    public int OrderIndex { get; private set; }

    /// <summary>
    /// Latitude du point de passage.
    /// </summary>
    public double Latitude { get; private set; }

    /// <summary>
    /// Longitude du point de passage.
    /// </summary>
    public double Longitude { get; private set; }

    /// <summary>
    /// Nom du point de passage.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Type de point de passage (Départ, Étape, Destination).
    /// </summary>
    public WaypointType Type { get; private set; }

    /// <summary>
    /// Identifiant de l'utilisateur ayant ajouté ce waypoint.
    /// </summary>
    public Guid AddedByUserId { get; private set; }

    /// <summary>
    /// Utilisateur ayant ajouté ce waypoint.
    /// </summary>
    public User AddedByUser { get; private set; } = null!;

    // Constructeur privé pour EF Core
    private TripWaypoint()
    {
    }

    /// <summary>
    /// Factory method pour créer un nouveau point de passage.
    /// </summary>
    /// <param name="tripId">Identifiant du voyage.</param>
    /// <param name="orderIndex">Index d'ordre.</param>
    /// <param name="latitude">Latitude [-90, 90].</param>
    /// <param name="longitude">Longitude [-180, 180].</param>
    /// <param name="name">Nom du point de passage.</param>
    /// <param name="type">Type de waypoint.</param>
    /// <param name="addedByUserId">Identifiant de l'utilisateur ajoutant le waypoint.</param>
    /// <returns>Nouvelle instance de TripWaypoint.</returns>
    public static TripWaypoint Create(
        Guid tripId,
        int orderIndex,
        double latitude,
        double longitude,
        string name,
        WaypointType type,
        Guid addedByUserId)
    {
        if (tripId == Guid.Empty)
            throw new ArgumentException("L'identifiant du voyage est invalide.", nameof(tripId));

        if (addedByUserId == Guid.Empty)
            throw new ArgumentException("L'identifiant de l'utilisateur est invalide.", nameof(addedByUserId));

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Le nom du point de passage est obligatoire.");

        if (latitude < -90 || latitude > 90)
            throw new DomainException("La latitude doit être comprise entre -90 et 90.");

        if (longitude < -180 || longitude > 180)
            throw new DomainException("La longitude doit être comprise entre -180 et 180.");

        return new TripWaypoint
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            OrderIndex = orderIndex,
            Latitude = latitude,
            Longitude = longitude,
            Name = name,
            Type = type,
            AddedByUserId = addedByUserId
        };
    }

    /// <summary>
    /// Met à jour l'index d'ordre du waypoint.
    /// </summary>
    /// <param name="newOrderIndex">Nouvel index d'ordre.</param>
    public void UpdateOrder(int newOrderIndex)
    {
        OrderIndex = newOrderIndex;
    }
}
