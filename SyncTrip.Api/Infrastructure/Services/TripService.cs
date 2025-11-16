using AutoMapper;
using SyncTrip.Api.Application.DTOs.Trips;
using SyncTrip.Api.Application.DTOs.Waypoints;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Enums;
using SyncTrip.Api.Core.Interfaces;

namespace SyncTrip.Api.Infrastructure.Services;

/// <summary>
/// Service de gestion des trips
/// </summary>
public class TripService : ITripService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TripService> _logger;

    public TripService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<TripService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TripDto> CreateTripAsync(Guid userId, CreateTripRequest request, CancellationToken cancellationToken = default)
    {
        // Vérifier que l'utilisateur est membre du convoi
        var isMember = await _unitOfWork.ConvoyParticipants.IsUserInConvoyAsync(userId, request.ConvoyId, cancellationToken);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("Vous devez être membre du convoi pour créer un trip");
        }

        // Vérifier qu'il n'y a pas déjà un trip actif (RÈGLE: un seul trip actif par convoi)
        var hasActiveTrip = await _unitOfWork.Trips.HasActiveTripAsync(request.ConvoyId, cancellationToken);
        if (hasActiveTrip)
        {
            throw new InvalidOperationException("Ce convoi a déjà un trip actif. Terminez-le avant d'en créer un nouveau.");
        }

        // Créer le trip
        var trip = _mapper.Map<Trip>(request);
        trip.Status = TripStatus.Planned;

        await _unitOfWork.Trips.AddAsync(trip, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Trip créé: {TripId} pour le convoi {ConvoyId}", trip.Id, request.ConvoyId);

        return _mapper.Map<TripDto>(trip);
    }

    public async Task<TripDto?> GetTripByIdAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        var trip = await _unitOfWork.Trips.GetTripWithWaypointsAsync(tripId, cancellationToken);
        return trip != null ? _mapper.Map<TripDto>(trip) : null;
    }

    public async Task<IEnumerable<TripDto>> GetConvoyTripsAsync(Guid convoyId, CancellationToken cancellationToken = default)
    {
        var trips = await _unitOfWork.Trips.GetConvoyTripsAsync(convoyId, cancellationToken);
        return _mapper.Map<IEnumerable<TripDto>>(trips);
    }

    public async Task<TripDto?> GetActiveConvoyTripAsync(Guid convoyId, CancellationToken cancellationToken = default)
    {
        var trip = await _unitOfWork.Trips.GetActiveConvoyTripAsync(convoyId, cancellationToken);
        return trip != null ? _mapper.Map<TripDto>(trip) : null;
    }

    public async Task<TripDto> UpdateTripStatusAsync(Guid tripId, UpdateTripStatusRequest request, CancellationToken cancellationToken = default)
    {
        var trip = await _unitOfWork.Trips.GetByIdAsync(tripId, cancellationToken);
        if (trip == null)
        {
            throw new InvalidOperationException("Trip non trouvé");
        }

        var oldStatus = trip.Status;
        trip.Status = request.Status;

        // Gérer les timestamps selon le statut
        switch (request.Status)
        {
            case TripStatus.InProgress when oldStatus == TripStatus.Planned:
                trip.ActualDepartureTime = DateTime.UtcNow;
                break;
            case TripStatus.Completed:
                trip.ActualArrivalTime = DateTime.UtcNow;
                break;
        }

        _unitOfWork.Trips.Update(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Trip {TripId} statut changé: {OldStatus} -> {NewStatus}", tripId, oldStatus, request.Status);

        return _mapper.Map<TripDto>(trip);
    }

    public async Task<WaypointDto> AddWaypointAsync(Guid tripId, CreateWaypointRequest request, CancellationToken cancellationToken = default)
    {
        // Vérifier que le trip existe
        var trip = await _unitOfWork.Trips.GetByIdAsync(tripId, cancellationToken);
        if (trip == null)
        {
            throw new InvalidOperationException("Trip non trouvé");
        }

        // Récupérer le nombre de waypoints existants pour calculer l'ordre
        var existingWaypoints = await _unitOfWork.Waypoints.GetTripWaypointsAsync(tripId, cancellationToken);
        var order = existingWaypoints.Count();

        // Créer le waypoint
        var waypoint = _mapper.Map<Waypoint>(request);
        waypoint.TripId = tripId;
        waypoint.Order = order;

        await _unitOfWork.Waypoints.AddAsync(waypoint, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Waypoint ajouté au trip {TripId}: {WaypointName}", tripId, waypoint.Name);

        return _mapper.Map<WaypointDto>(waypoint);
    }

    public async Task MarkWaypointReachedAsync(Guid waypointId, CancellationToken cancellationToken = default)
    {
        var waypoint = await _unitOfWork.Waypoints.GetByIdAsync(waypointId, cancellationToken);
        if (waypoint == null)
        {
            throw new InvalidOperationException("Waypoint non trouvé");
        }

        if (waypoint.IsReached)
        {
            throw new InvalidOperationException("Ce waypoint a déjà été atteint");
        }

        waypoint.IsReached = true;
        waypoint.ReachedAt = DateTime.UtcNow;

        _unitOfWork.Waypoints.Update(waypoint);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Waypoint {WaypointId} marqué comme atteint", waypointId);
    }
}
