using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Trips.Commands;

/// <summary>
/// Handler pour ajouter un waypoint à un voyage.
/// </summary>
public class AddWaypointCommandHandler : IRequestHandler<AddWaypointCommand, Guid>
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<AddWaypointCommandHandler> _logger;

    public AddWaypointCommandHandler(
        ITripRepository tripRepository,
        ILogger<AddWaypointCommandHandler> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(AddWaypointCommand request, CancellationToken cancellationToken)
    {
        // Récupérer le voyage
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);
        if (trip == null)
            throw new KeyNotFoundException($"Voyage avec l'ID {request.TripId} introuvable.");

        // Vérifier que l'utilisateur est membre du convoi
        if (!trip.Convoy.IsMember(request.UserId))
            throw new UnauthorizedAccessException("Vous n'êtes pas membre de ce convoi.");

        // Ajouter le waypoint
        var waypoint = trip.AddWaypoint(request.OrderIndex, request.Latitude, request.Longitude, request.Name, request.Type, request.UserId);

        await _tripRepository.UpdateAsync(trip, cancellationToken);

        _logger.LogInformation("Waypoint {WaypointId} ajouté au voyage {TripId} par {UserId}",
            waypoint.Id, request.TripId, request.UserId);

        return waypoint.Id;
    }
}
