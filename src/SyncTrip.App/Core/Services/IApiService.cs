namespace SyncTrip.App.Core.Services;

public interface IApiService
{
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken ct = default);
    Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken ct = default);
    Task<bool> PostAsync<TRequest>(string endpoint, TRequest request, CancellationToken ct = default);
    Task<bool> PutAsync<TRequest>(string endpoint, TRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(string endpoint, CancellationToken ct = default);
}
