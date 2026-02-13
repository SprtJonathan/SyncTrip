namespace SyncTrip.Shared.DTOs.Navigation;

public class RouteResultDto
{
    public string GeometryGeoJson { get; init; } = string.Empty;
    public double DistanceMeters { get; init; }
    public double DurationSeconds { get; init; }
    public List<RouteStepDto> Steps { get; init; } = new();
}

public class RouteStepDto
{
    public string Instruction { get; init; } = string.Empty;
    public double DistanceMeters { get; init; }
    public double DurationSeconds { get; init; }
    public string Name { get; init; } = string.Empty;
}
