using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Trips.Commands;

/// <summary>
/// Handler pour démarrer un nouveau voyage.
/// </summary>
public class StartTripCommandHandler : IRequestHandler<StartTripCommand, Guid>
{
    private readonly ITripRepository _tripRepository;
    private readonly IConvoyRepository _convoyRepository;
    private readonly ILogger<StartTripCommandHandler> _logger;

    public StartTripCommandHandler(
        ITripRepository tripRepository,
        IConvoyRepository convoyRepository,
        ILogger<StartTripCommandHandler> logger)
    {
        _tripRepository = tripRepository;
        _convoyRepository = convoyRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(StartTripCommand request, CancellationToken cancellationToken)
    {
        // Vérifier que le convoi existe
        var convoy = await _convoyRepository.GetByIdAsync(request.ConvoyId, cancellationToken);
        if (convoy == null)
            throw new KeyNotFoundException($"Convoi avec l'ID {request.ConvoyId} introuvable.");

        // Vérifier que l'utilisateur est le leader
        convoy.EnsureIsLeader(request.UserId);

        // Vérifier qu'il n'y a pas de voyage actif
        var activeTrip = await _tripRepository.GetActiveByConvoyIdAsync(request.ConvoyId, cancellationToken);
        if (activeTrip != null)
            throw new InvalidOperationException("Un voyage est déjà en cours pour ce convoi.");

        // Créer le voyage
        var trip = Trip.Create(request.ConvoyId, request.Status, request.RouteProfile);

        // Ajouter les waypoints initiaux
        foreach (var wp in request.Waypoints)
        {
            trip.AddWaypoint(wp.OrderIndex, wp.Latitude, wp.Longitude, wp.Name, (WaypointType)wp.Type, request.UserId);
        }

        await _tripRepository.AddAsync(trip, cancellationToken);

        _logger.LogInformation("Voyage {TripId} démarré pour le convoi {ConvoyId} par {UserId}",
            trip.Id, request.ConvoyId, request.UserId);

        return trip.Id;
    }
}
