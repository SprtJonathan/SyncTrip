using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Infrastructure.Services;

public class OsrmRoutingService : IRoutingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OsrmRoutingService> _logger;
    private const string BaseUrl = "https://router.project-osrm.org";

    public OsrmRoutingService(HttpClient httpClient, ILogger<OsrmRoutingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<RouteResult> CalculateRouteAsync(
        IList<(double Latitude, double Longitude)> waypoints,
        RouteProfile profile,
        CancellationToken ct = default)
    {
        if (waypoints.Count < 2)
            throw new ArgumentException("Au moins 2 waypoints sont necessaires.");

        var coords = string.Join(";", waypoints.Select(w =>
            FormattableString.Invariant($"{w.Longitude},{w.Latitude}")));

        var url = $"{BaseUrl}/route/v1/driving/{coords}?overview=full&geometries=geojson&steps=true";

        if (profile == RouteProfile.Scenic)
            url += "&exclude=motorway";

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex) when (profile == RouteProfile.Scenic)
        {
            _logger.LogWarning(ex, "OSRM exclude=motorway non supporte, fallback sur itineraire standard");
            var fallbackUrl = $"{BaseUrl}/route/v1/driving/{coords}?overview=full&geometries=geojson&steps=true";
            response = await _httpClient.GetAsync(fallbackUrl, ct);
            response.EnsureSuccessStatusCode();
        }

        var content = await response.Content.ReadAsStringAsync(ct);
        var osrmResponse = JsonSerializer.Deserialize<OsrmResponse>(content)
            ?? throw new InvalidOperationException("Reponse OSRM invalide.");

        if (osrmResponse.Code != "Ok" || osrmResponse.Routes.Count == 0)
            throw new InvalidOperationException($"OSRM erreur : {osrmResponse.Code}");

        var route = osrmResponse.Routes[0];
        var geometryJson = JsonSerializer.Serialize(route.Geometry);

        var steps = route.Legs
            .SelectMany(leg => leg.Steps)
            .Select(step => new RouteStep(
                Instruction: step.Maneuver?.Type ?? string.Empty,
                DistanceMeters: step.Distance,
                DurationSeconds: step.Duration,
                Name: step.Name))
            .ToList();

        return new RouteResult(
            GeometryGeoJson: geometryJson,
            DistanceMeters: route.Distance,
            DurationSeconds: route.Duration,
            Steps: steps);
    }

    private record OsrmResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; init; } = string.Empty;

        [JsonPropertyName("routes")]
        public List<OsrmRoute> Routes { get; init; } = [];
    }

    private record OsrmRoute
    {
        [JsonPropertyName("distance")]
        public double Distance { get; init; }

        [JsonPropertyName("duration")]
        public double Duration { get; init; }

        [JsonPropertyName("geometry")]
        public JsonElement Geometry { get; init; }

        [JsonPropertyName("legs")]
        public List<OsrmLeg> Legs { get; init; } = [];
    }

    private record OsrmLeg
    {
        [JsonPropertyName("steps")]
        public List<OsrmStep> Steps { get; init; } = [];
    }

    private record OsrmStep
    {
        [JsonPropertyName("distance")]
        public double Distance { get; init; }

        [JsonPropertyName("duration")]
        public double Duration { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("maneuver")]
        public OsrmManeuver? Maneuver { get; init; }
    }

    private record OsrmManeuver
    {
        [JsonPropertyName("type")]
        public string Type { get; init; } = string.Empty;
    }
}
