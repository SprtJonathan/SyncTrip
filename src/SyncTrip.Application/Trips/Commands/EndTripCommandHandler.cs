using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Trips.Commands;

/// <summary>
/// Handler pour terminer un voyage.
/// </summary>
public class EndTripCommandHandler : IRequestHandler<EndTripCommand>
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<EndTripCommandHandler> _logger;

    public EndTripCommandHandler(
        ITripRepository tripRepository,
        ILogger<EndTripCommandHandler> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task Handle(EndTripCommand request, CancellationToken cancellationToken)
    {
        // Récupérer le voyage
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);
        if (trip == null)
            throw new KeyNotFoundException($"Voyage avec l'ID {request.TripId} introuvable.");

        // Vérifier que l'utilisateur est le leader du convoi
        trip.Convoy.EnsureIsLeader(request.UserId);

        // Terminer le voyage
        trip.Finish();

        await _tripRepository.UpdateAsync(trip, cancellationToken);

        _logger.LogInformation("Voyage {TripId} terminé par {UserId}", request.TripId, request.UserId);
    }
}
