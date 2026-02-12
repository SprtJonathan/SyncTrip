using MediatR;
using SyncTrip.Core.Enums;

namespace SyncTrip.Application.Voting.Commands;

/// <summary>
/// Command pour proposer un arrêt lors d'un voyage.
/// </summary>
public record ProposeStopCommand : IRequest<Guid>
{
    /// <summary>
    /// Identifiant du voyage.
    /// </summary>
    public Guid TripId { get; init; }

    /// <summary>
    /// Identifiant de l'utilisateur proposant l'arrêt.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Type d'arrêt proposé.
    /// </summary>
    public StopType StopType { get; init; }

    /// <summary>
    /// Latitude de l'arrêt.
    /// </summary>
    public double Latitude { get; init; }

    /// <summary>
    /// Longitude de l'arrêt.
    /// </summary>
    public double Longitude { get; init; }

    /// <summary>
    /// Nom du lieu de l'arrêt.
    /// </summary>
    public string LocationName { get; init; } = string.Empty;
}
