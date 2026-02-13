namespace SyncTrip.Shared.DTOs.Navigation;

public record CalculateRouteRequest
{
    public int RouteProfile { get; init; }
    public List<WaypointCoordinate> Waypoints { get; init; } = new();
}

public record WaypointCoordinate
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}
