using AutoMapper;
using SyncTrip.Api.Application.DTOs.Locations;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Interfaces;

namespace SyncTrip.Api.Infrastructure.Services;

/// <summary>
/// Service de gestion des positions GPS
/// </summary>
public class LocationService : ILocationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<LocationService> _logger;

    public LocationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<LocationService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<LocationDto> UpdateLocationAsync(Guid userId, Guid tripId, UpdateLocationRequest request, CancellationToken cancellationToken = default)
    {
        // Vérifier que le trip existe
        var trip = await _unitOfWork.Trips.GetByIdAsync(tripId, cancellationToken);
        if (trip == null)
        {
            throw new InvalidOperationException("Trip non trouvé");
        }

        // Vérifier que l'utilisateur est membre du convoi
        var isMember = await _unitOfWork.ConvoyParticipants.IsUserInConvoyAsync(userId, trip.ConvoyId, cancellationToken);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("Vous devez être membre du convoi pour partager votre position");
        }

        // Créer l'entrée d'historique de localisation
        var location = _mapper.Map<LocationHistory>(request);
        location.TripId = tripId;
        location.UserId = userId;
        location.Timestamp = DateTime.UtcNow;

        await _unitOfWork.LocationHistories.AddAsync(location, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Position mise à jour pour l'utilisateur {UserId} dans le trip {TripId}", userId, tripId);

        // Récupérer la location complète avec l'utilisateur
        var savedLocation = await _unitOfWork.LocationHistories.GetByIdAsync(location.Id, cancellationToken);
        return _mapper.Map<LocationDto>(savedLocation);
    }

    public async Task<LocationDto?> GetLastUserLocationAsync(Guid userId, Guid tripId, CancellationToken cancellationToken = default)
    {
        var location = await _unitOfWork.LocationHistories.GetLastUserLocationAsync(userId, tripId, cancellationToken);
        return location != null ? _mapper.Map<LocationDto>(location) : null;
    }

    public async Task<IEnumerable<LocationDto>> GetTripParticipantsLocationsAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        var locations = await _unitOfWork.LocationHistories.GetTripParticipantsLastLocationsAsync(tripId, cancellationToken);
        return _mapper.Map<IEnumerable<LocationDto>>(locations);
    }
}
