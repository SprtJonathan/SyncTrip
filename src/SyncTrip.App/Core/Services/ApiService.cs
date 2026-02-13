using System.Net.Http.Json;
using System.Text.Json;

namespace SyncTrip.App.Core.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, request, ct);
        response.EnsureSuccessStatusCode();
        return await ReadResponseAsync<TResponse>(response, ct);
    }

    public async Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync(endpoint, ct);
        response.EnsureSuccessStatusCode();
        return await ReadResponseAsync<TResponse>(response, ct);
    }

    public async Task<bool> PostAsync<TRequest>(string endpoint, TRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, request, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PutAsync<TRequest>(string endpoint, TRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PutAsJsonAsync(endpoint, request, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string endpoint, CancellationToken ct = default)
    {
        var response = await _httpClient.DeleteAsync(endpoint, ct);
        return response.IsSuccessStatusCode;
    }

    private async Task<TResponse?> ReadResponseAsync<TResponse>(HttpResponseMessage response, CancellationToken ct)
    {
        var content = await response.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(content))
            return default;

        // For Guid type, extract from wrapped response like { "convoyId": "guid" }
        if (typeof(TResponse) == typeof(Guid))
        {
            using var doc = JsonDocument.Parse(content);
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.String &&
                    Guid.TryParse(prop.Value.GetString(), out var guid))
                {
                    return (TResponse)(object)guid;
                }
            }
            return default;
        }

        return JsonSerializer.Deserialize<TResponse>(content, _jsonOptions);
    }
}
