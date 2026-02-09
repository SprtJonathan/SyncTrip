using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;

namespace SyncTrip.Core.Entities;

/// <summary>
/// Représente un voyage GPS au sein d'un convoi.
/// </summary>
public class Trip
{
    /// <summary>
    /// Identifiant unique du voyage.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identifiant du convoi associé.
    /// </summary>
    public Guid ConvoyId { get; private set; }

    /// <summary>
    /// Convoi associé.
    /// </summary>
    public Convoy Convoy { get; private set; } = null!;

    /// <summary>
    /// Statut du voyage.
    /// </summary>
    public TripStatus Status { get; private set; }

    /// <summary>
    /// Date/heure de début du voyage.
    /// </summary>
    public DateTime StartTime { get; private set; }

    /// <summary>
    /// Date/heure de fin du voyage (null si en cours).
    /// </summary>
    public DateTime? EndTime { get; private set; }

    /// <summary>
    /// Profil de route utilisé.
    /// </summary>
    public RouteProfile RouteProfile { get; private set; }

    /// <summary>
    /// Collection des points de passage du voyage.
    /// </summary>
    public ICollection<TripWaypoint> Waypoints { get; private set; } = new List<TripWaypoint>();

    // Constructeur privé pour EF Core
    private Trip()
    {
    }

    /// <summary>
    /// Factory method pour créer un nouveau voyage.
    /// </summary>
    /// <param name="convoyId">Identifiant du convoi.</param>
    /// <param name="status">Statut initial (Recording ou MonitorOnly).</param>
    /// <param name="routeProfile">Profil de route.</param>
    /// <returns>Nouvelle instance de Trip.</returns>
    public static Trip Create(Guid convoyId, TripStatus status, RouteProfile routeProfile)
    {
        if (convoyId == Guid.Empty)
            throw new ArgumentException("L'identifiant du convoi est invalide.", nameof(convoyId));

        if (status == TripStatus.Finished)
            throw new DomainException("Un voyage ne peut pas être créé avec le statut Terminé.");

        return new Trip
        {
            Id = Guid.NewGuid(),
            ConvoyId = convoyId,
            Status = status,
            StartTime = DateTime.UtcNow,
            EndTime = null,
            RouteProfile = routeProfile
        };
    }

    /// <summary>
    /// Termine le voyage.
    /// </summary>
    /// <exception cref="DomainException">Si le voyage est déjà terminé.</exception>
    public void Finish()
    {
        if (Status == TripStatus.Finished)
            throw new DomainException("Ce voyage est déjà terminé.");

        Status = TripStatus.Finished;
        EndTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Ajoute un point de passage au voyage.
    /// </summary>
    /// <param name="orderIndex">Index d'ordre.</param>
    /// <param name="latitude">Latitude.</param>
    /// <param name="longitude">Longitude.</param>
    /// <param name="name">Nom du waypoint.</param>
    /// <param name="type">Type de waypoint.</param>
    /// <param name="addedByUserId">Utilisateur ajoutant le waypoint.</param>
    /// <returns>Le waypoint créé.</returns>
    /// <exception cref="DomainException">Si le voyage est terminé.</exception>
    public TripWaypoint AddWaypoint(int orderIndex, double latitude, double longitude, string name, WaypointType type, Guid addedByUserId)
    {
        if (Status == TripStatus.Finished)
            throw new DomainException("Impossible d'ajouter un waypoint à un voyage terminé.");

        var waypoint = TripWaypoint.Create(Id, orderIndex, latitude, longitude, name, type, addedByUserId);
        Waypoints.Add(waypoint);
        return waypoint;
    }

    /// <summary>
    /// Supprime un point de passage du voyage.
    /// </summary>
    /// <param name="waypointId">Identifiant du waypoint à supprimer.</param>
    /// <exception cref="DomainException">Si le voyage est terminé ou si le waypoint n'existe pas.</exception>
    public void RemoveWaypoint(Guid waypointId)
    {
        if (Status == TripStatus.Finished)
            throw new DomainException("Impossible de supprimer un waypoint d'un voyage terminé.");

        var waypoint = Waypoints.FirstOrDefault(w => w.Id == waypointId);
        if (waypoint == null)
            throw new DomainException("Ce waypoint n'existe pas dans ce voyage.");

        Waypoints.Remove(waypoint);
    }
}
