using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.Application.Trips.Queries;

/// <summary>
/// Handler pour récupérer l'historique des voyages d'un convoi.
/// </summary>
public class GetConvoyTripsQueryHandler : IRequestHandler<GetConvoyTripsQuery, IList<TripDto>>
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<GetConvoyTripsQueryHandler> _logger;

    public GetConvoyTripsQueryHandler(
        ITripRepository tripRepository,
        ILogger<GetConvoyTripsQueryHandler> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task<IList<TripDto>> Handle(GetConvoyTripsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Récupération des voyages du convoi {ConvoyId}", request.ConvoyId);

        var trips = await _tripRepository.GetByConvoyIdAsync(request.ConvoyId, cancellationToken);

        return trips
            .OrderByDescending(t => t.StartTime)
            .Select(t => new TripDto
            {
                Id = t.Id,
                ConvoyId = t.ConvoyId,
                Status = (int)t.Status,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                RouteProfile = (int)t.RouteProfile,
                WaypointCount = t.Waypoints.Count
            })
            .ToList();
    }
}
