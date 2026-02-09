using MediatR;
using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.Application.Trips.Queries;

/// <summary>
/// Query pour récupérer un voyage par son identifiant.
/// </summary>
public record GetTripByIdQuery : IRequest<TripDetailsDto?>
{
    /// <summary>
    /// Identifiant du voyage.
    /// </summary>
    public Guid TripId { get; init; }

    public GetTripByIdQuery(Guid tripId)
    {
        TripId = tripId;
    }
}
