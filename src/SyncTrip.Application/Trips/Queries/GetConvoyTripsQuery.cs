using MediatR;
using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.Application.Trips.Queries;

/// <summary>
/// Query pour récupérer l'historique des voyages d'un convoi.
/// </summary>
public record GetConvoyTripsQuery : IRequest<IList<TripDto>>
{
    /// <summary>
    /// Identifiant du convoi.
    /// </summary>
    public Guid ConvoyId { get; init; }

    public GetConvoyTripsQuery(Guid convoyId)
    {
        ConvoyId = convoyId;
    }
}
