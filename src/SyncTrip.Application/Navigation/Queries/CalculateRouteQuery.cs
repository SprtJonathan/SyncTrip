using MediatR;
using SyncTrip.Core.Enums;
using SyncTrip.Shared.DTOs.Navigation;

namespace SyncTrip.Application.Navigation.Queries;

public record CalculateRouteQuery : IRequest<RouteResultDto>
{
    public RouteProfile RouteProfile { get; init; }
    public List<WaypointCoordinate> Waypoints { get; init; } = new();
}
