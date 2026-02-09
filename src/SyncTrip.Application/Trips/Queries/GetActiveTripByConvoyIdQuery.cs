using MediatR;
using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.Application.Trips.Queries;

/// <summary>
/// Query pour récupérer le voyage actif d'un convoi.
/// </summary>
public record GetActiveTripByConvoyIdQuery : IRequest<TripDetailsDto?>
{
    /// <summary>
    /// Identifiant du convoi.
    /// </summary>
    public Guid ConvoyId { get; init; }

    public GetActiveTripByConvoyIdQuery(Guid convoyId)
    {
        ConvoyId = convoyId;
    }
}
