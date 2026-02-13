using System.Net.Http.Json;
using System.Text.Json;

namespace SyncTrip.Mobile.Core.Services;

/// <summary>
/// Implémentation du service de communication avec l'API backend.
/// Gère les appels HTTP avec sérialisation/désérialisation JSON automatique.
/// </summary>
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initialise une nouvelle instance du service API.
    /// </summary>
    /// <param name="httpClient">Client HTTP configuré avec l'URL de base.</param>
    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc />
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, ct);
    }

    /// <inheritdoc />
    public async Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync(endpoint, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, ct);
    }

    /// <inheritdoc />
    public async Task<bool> PostAsync<TRequest>(string endpoint, TRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, request, ct);
        return response.IsSuccessStatusCode;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string endpoint, CancellationToken ct = default)
    {
        var response = await _httpClient.DeleteAsync(endpoint, ct);
        return response.IsSuccessStatusCode;
    }
}
