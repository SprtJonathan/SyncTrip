using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Infrastructure.Services;

public class NominatimGeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NominatimGeocodingService> _logger;
    private const string BaseUrl = "https://nominatim.openstreetmap.org";

    public NominatimGeocodingService(HttpClient httpClient, ILogger<NominatimGeocodingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IList<GeocodingResult>> SearchAsync(string query, int limit = 5, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/search?q={Uri.EscapeDataString(query)}&format=jsonv2&limit={limit}&addressdetails=1";

        var response = await _httpClient.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();

        var results = await response.Content.ReadFromJsonAsync<List<NominatimResult>>(ct) ?? [];

        return results.Select(r => new GeocodingResult(
            DisplayName: r.DisplayName,
            Latitude: double.Parse(r.Lat, System.Globalization.CultureInfo.InvariantCulture),
            Longitude: double.Parse(r.Lon, System.Globalization.CultureInfo.InvariantCulture),
            Type: r.Type,
            Importance: r.Importance
        )).ToList();
    }

    private record NominatimResult
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; init; } = string.Empty;

        [JsonPropertyName("lat")]
        public string Lat { get; init; } = "0";

        [JsonPropertyName("lon")]
        public string Lon { get; init; } = "0";

        [JsonPropertyName("type")]
        public string Type { get; init; } = string.Empty;

        [JsonPropertyName("importance")]
        public double Importance { get; init; }
    }
}
