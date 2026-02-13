namespace SyncTrip.Core.Interfaces;

public interface IGeocodingService
{
    Task<IList<GeocodingResult>> SearchAsync(string query, int limit = 5, CancellationToken ct = default);
}

public record GeocodingResult(
    string DisplayName,
    double Latitude,
    double Longitude,
    string Type,
    double Importance);
