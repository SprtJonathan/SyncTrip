using MediatR;
using SyncTrip.Shared.DTOs.Navigation;

namespace SyncTrip.Application.Navigation.Commands;

public record CalculateTripRouteCommand : IRequest<RouteResultDto>
{
    public Guid TripId { get; init; }
    public Guid UserId { get; init; }
}
