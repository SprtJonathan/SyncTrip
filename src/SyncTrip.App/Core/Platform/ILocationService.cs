namespace SyncTrip.App.Core.Platform;

public class LocationResult
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}

public interface ILocationService
{
    Task<LocationResult?> GetCurrentLocationAsync();
}
