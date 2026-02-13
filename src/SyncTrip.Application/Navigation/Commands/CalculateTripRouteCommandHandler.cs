using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Navigation;

namespace SyncTrip.Application.Navigation.Commands;

public class CalculateTripRouteCommandHandler : IRequestHandler<CalculateTripRouteCommand, RouteResultDto>
{
    private readonly ITripRepository _tripRepository;
    private readonly IRoutingService _routingService;
    private readonly ILogger<CalculateTripRouteCommandHandler> _logger;

    public CalculateTripRouteCommandHandler(
        ITripRepository tripRepository,
        IRoutingService routingService,
        ILogger<CalculateTripRouteCommandHandler> logger)
    {
        _tripRepository = tripRepository;
        _routingService = routingService;
        _logger = logger;
    }

    public async Task<RouteResultDto> Handle(CalculateTripRouteCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);
        if (trip == null)
            throw new KeyNotFoundException($"Voyage avec l'ID {request.TripId} introuvable.");

        if (!trip.Convoy.IsMember(request.UserId))
            throw new UnauthorizedAccessException("Vous n'etes pas membre de ce convoi.");

        var orderedWaypoints = trip.Waypoints
            .OrderBy(w => w.OrderIndex)
            .Select(w => (w.Latitude, w.Longitude))
            .ToList();

        if (orderedWaypoints.Count < 2)
            throw new InvalidOperationException("Au moins 2 waypoints sont necessaires pour calculer un itineraire.");

        var result = await _routingService.CalculateRouteAsync(
            orderedWaypoints, trip.RouteProfile, cancellationToken);

        trip.UpdateRoute(result.GeometryGeoJson, result.DistanceMeters, result.DurationSeconds);
        await _tripRepository.UpdateAsync(trip, cancellationToken);

        _logger.LogInformation("Itineraire calcule pour le voyage {TripId}: {Distance}m, {Duration}s",
            request.TripId, result.DistanceMeters, result.DurationSeconds);

        return new RouteResultDto
        {
            GeometryGeoJson = result.GeometryGeoJson,
            DistanceMeters = result.DistanceMeters,
            DurationSeconds = result.DurationSeconds,
            Steps = result.Steps.Select(s => new RouteStepDto
            {
                Instruction = s.Instruction,
                DistanceMeters = s.DistanceMeters,
                DurationSeconds = s.DurationSeconds,
                Name = s.Name
            }).ToList()
        };
    }
}
