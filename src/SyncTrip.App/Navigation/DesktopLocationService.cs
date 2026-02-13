using System.Net.Http.Json;
using System.Text.Json.Serialization;
using SyncTrip.App.Core.Platform;

namespace SyncTrip.App.Navigation;

public class DesktopLocationService : ILocationService
{
    private readonly HttpClient _httpClient;
    private LocationResult? _cachedLocation;
    private DateTime _cacheTime = DateTime.MinValue;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

    public DesktopLocationService()
    {
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
    }

    public async Task<LocationResult?> GetCurrentLocationAsync()
    {
        if (_cachedLocation is not null && DateTime.UtcNow - _cacheTime < CacheDuration)
            return _cachedLocation;

        try
        {
            var response = await _httpClient.GetFromJsonAsync<IpLocationResponse>("http://ip-api.com/json/?fields=lat,lon,status");
            if (response is { Status: "success" })
            {
                _cachedLocation = new LocationResult
                {
                    Latitude = response.Lat,
                    Longitude = response.Lon
                };
                _cacheTime = DateTime.UtcNow;
            }
        }
        catch
        {
            if (_cachedLocation is null)
            {
                _cachedLocation = new LocationResult
                {
                    Latitude = 48.8566,
                    Longitude = 2.3522
                };
                _cacheTime = DateTime.UtcNow;
            }
        }

        return _cachedLocation;
    }

    private class IpLocationResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }
}
