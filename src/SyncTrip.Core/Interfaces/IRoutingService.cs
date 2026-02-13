using SyncTrip.Core.Enums;

namespace SyncTrip.Core.Interfaces;

public interface IRoutingService
{
    Task<RouteResult> CalculateRouteAsync(
        IList<(double Latitude, double Longitude)> waypoints,
        RouteProfile profile,
        CancellationToken ct = default);
}

public record RouteResult(
    string GeometryGeoJson,
    double DistanceMeters,
    double DurationSeconds,
    IList<RouteStep> Steps);

public record RouteStep(
    string Instruction,
    double DistanceMeters,
    double DurationSeconds,
    string Name);
