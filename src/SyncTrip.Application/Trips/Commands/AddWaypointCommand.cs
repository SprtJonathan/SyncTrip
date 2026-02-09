using MediatR;
using SyncTrip.Core.Enums;

namespace SyncTrip.Application.Trips.Commands;

/// <summary>
/// Command pour ajouter un waypoint à un voyage.
/// </summary>
public record AddWaypointCommand : IRequest<Guid>
{
    /// <summary>
    /// Identifiant du voyage.
    /// </summary>
    public Guid TripId { get; init; }

    /// <summary>
    /// Identifiant de l'utilisateur.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Index d'ordre.
    /// </summary>
    public int OrderIndex { get; init; }

    /// <summary>
    /// Latitude.
    /// </summary>
    public double Latitude { get; init; }

    /// <summary>
    /// Longitude.
    /// </summary>
    public double Longitude { get; init; }

    /// <summary>
    /// Nom du waypoint.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Type de waypoint.
    /// </summary>
    public WaypointType Type { get; init; }
}
