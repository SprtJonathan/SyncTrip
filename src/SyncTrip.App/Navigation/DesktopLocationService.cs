using System.Net.Http.Json;
using System.Text.Json.Serialization;
using SyncTrip.App.Core.Platform;

namespace SyncTrip.App.Navigation;

public class DesktopLocationService : ILocationService
{
    private readonly HttpClient _httpClient;
    private LocationResult? _cachedLocation;
    private bool _fetchAttempted;

    public DesktopLocationService()
    {
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
    }

    public async Task<LocationResult?> GetCurrentLocationAsync()
    {
        if (_cachedLocation is not null)
            return _cachedLocation;

        if (_fetchAttempted)
            return _cachedLocation;

        _fetchAttempted = true;

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
            }
        }
        catch
        {
            // Geolocalisation IP indisponible — position par defaut (Paris)
            _cachedLocation = new LocationResult
            {
                Latitude = 48.8566,
                Longitude = 2.3522
            };
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
