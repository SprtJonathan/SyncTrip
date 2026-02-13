using Microsoft.AspNetCore.SignalR.Client;

namespace SyncTrip.Mobile.Core.Services;

/// <summary>
/// Implémentation du service SignalR pour le temps réel GPS.
/// </summary>
public class SignalRService : ISignalRService
{
    private readonly IAuthenticationService _authService;
    private HubConnection? _hubConnection;
    private Guid _currentTripId;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public event Action<string, double, double, DateTime>? LocationReceived;

    public SignalRService(IAuthenticationService authService)
    {
        _authService = authService;
    }

    /// <inheritdoc />
    public async Task ConnectAsync(Guid tripId)
    {
        if (IsConnected)
            await DisconnectAsync();

        var token = await _authService.GetTokenAsync();

        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"http://localhost:5000/hubs/trip", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<object>("ReceiveLocationUpdate", data =>
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(data);
                var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;

                var userId = root.GetProperty("UserId").GetString() ?? string.Empty;
                var lat = root.GetProperty("Latitude").GetDouble();
                var lon = root.GetProperty("Longitude").GetDouble();
                var timestamp = root.GetProperty("Timestamp").GetDateTime();

                LocationReceived?.Invoke(userId, lat, lon, timestamp);
            }
            catch
            {
                // Ignorer les messages malformés
            }
        });

        await _hubConnection.StartAsync();

        _currentTripId = tripId;
        await _hubConnection.InvokeAsync("JoinTrip", tripId);
    }

    /// <inheritdoc />
    public async Task SendLocationAsync(Guid tripId, double latitude, double longitude)
    {
        if (_hubConnection is not { State: HubConnectionState.Connected })
            return;

        await _hubConnection.InvokeAsync("SendLocationUpdate", tripId, latitude, longitude);
    }

    /// <inheritdoc />
    public async Task DisconnectAsync()
    {
        if (_hubConnection is null)
            return;

        try
        {
            if (_hubConnection.State == HubConnectionState.Connected)
                await _hubConnection.InvokeAsync("LeaveTrip", _currentTripId);

            await _hubConnection.StopAsync();
        }
        finally
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }
}
