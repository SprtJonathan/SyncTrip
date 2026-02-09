using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.Application.Trips.Queries;

/// <summary>
/// Handler pour récupérer le voyage actif d'un convoi.
/// </summary>
public class GetActiveTripByConvoyIdQueryHandler : IRequestHandler<GetActiveTripByConvoyIdQuery, TripDetailsDto?>
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<GetActiveTripByConvoyIdQueryHandler> _logger;

    public GetActiveTripByConvoyIdQueryHandler(
        ITripRepository tripRepository,
        ILogger<GetActiveTripByConvoyIdQueryHandler> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task<TripDetailsDto?> Handle(GetActiveTripByConvoyIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Récupération du voyage actif du convoi {ConvoyId}", request.ConvoyId);

        var trip = await _tripRepository.GetActiveByConvoyIdAsync(request.ConvoyId, cancellationToken);
        if (trip == null)
            return null;

        return new TripDetailsDto
        {
            Id = trip.Id,
            ConvoyId = trip.ConvoyId,
            Status = (int)trip.Status,
            StartTime = trip.StartTime,
            EndTime = trip.EndTime,
            RouteProfile = (int)trip.RouteProfile,
            WaypointCount = trip.Waypoints.Count,
            Waypoints = trip.Waypoints
                .OrderBy(w => w.OrderIndex)
                .Select(w => new TripWaypointDto
                {
                    Id = w.Id,
                    OrderIndex = w.OrderIndex,
                    Latitude = w.Latitude,
                    Longitude = w.Longitude,
                    Name = w.Name,
                    Type = (int)w.Type,
                    AddedByUserId = w.AddedByUserId,
                    AddedByUsername = w.AddedByUser?.Username ?? string.Empty
                })
                .ToList()
        };
    }
}
