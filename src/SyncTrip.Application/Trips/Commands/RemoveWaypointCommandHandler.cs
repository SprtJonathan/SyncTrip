using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Trips.Commands;

/// <summary>
/// Handler pour supprimer un waypoint d'un voyage.
/// </summary>
public class RemoveWaypointCommandHandler : IRequestHandler<RemoveWaypointCommand>
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<RemoveWaypointCommandHandler> _logger;

    public RemoveWaypointCommandHandler(
        ITripRepository tripRepository,
        ILogger<RemoveWaypointCommandHandler> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task Handle(RemoveWaypointCommand request, CancellationToken cancellationToken)
    {
        // Récupérer le voyage
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);
        if (trip == null)
            throw new KeyNotFoundException($"Voyage avec l'ID {request.TripId} introuvable.");

        // Vérifier que l'utilisateur est le leader du convoi
        trip.Convoy.EnsureIsLeader(request.UserId);

        // Supprimer le waypoint
        trip.RemoveWaypoint(request.WaypointId);

        await _tripRepository.UpdateAsync(trip, cancellationToken);

        _logger.LogInformation("Waypoint {WaypointId} supprimé du voyage {TripId} par {UserId}",
            request.WaypointId, request.TripId, request.UserId);
    }
}
