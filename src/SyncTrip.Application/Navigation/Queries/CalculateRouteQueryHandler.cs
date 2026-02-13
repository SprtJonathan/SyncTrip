using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Navigation;

namespace SyncTrip.Application.Navigation.Queries;

public class CalculateRouteQueryHandler : IRequestHandler<CalculateRouteQuery, RouteResultDto>
{
    private readonly IRoutingService _routingService;
    private readonly ILogger<CalculateRouteQueryHandler> _logger;

    public CalculateRouteQueryHandler(IRoutingService routingService, ILogger<CalculateRouteQueryHandler> logger)
    {
        _routingService = routingService;
        _logger = logger;
    }

    public async Task<RouteResultDto> Handle(CalculateRouteQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calcul d'itineraire avec {Count} waypoints, profil {Profile}",
            request.Waypoints.Count, request.RouteProfile);

        var waypoints = request.Waypoints
            .Select(w => (w.Latitude, w.Longitude))
            .ToList();

        var result = await _routingService.CalculateRouteAsync(waypoints, request.RouteProfile, cancellationToken);

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
