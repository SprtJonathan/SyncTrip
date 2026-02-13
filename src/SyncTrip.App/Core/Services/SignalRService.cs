using Microsoft.AspNetCore.SignalR.Client;
using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.App.Core.Services;

public class SignalRService : ISignalRService
{
    private readonly IAuthenticationService _authService;
    private HubConnection? _hubConnection;
    private Guid _currentTripId;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public event Action<string, double, double, DateTime>? LocationReceived;
    public event Action<StopProposalDto>? StopProposed;
    public event Action<Guid, int, int>? VoteUpdated;
    public event Action<StopProposalDto>? ProposalResolved;

    public SignalRService(IAuthenticationService authService)
    {
        _authService = authService;
    }

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
                // Ignorer les messages malformes
            }
        });

        _hubConnection.On<StopProposalDto>("StopProposed", proposal =>
        {
            StopProposed?.Invoke(proposal);
        });

        _hubConnection.On<object>("VoteUpdate", data =>
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(data);
                var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;

                var proposalId = root.GetProperty("ProposalId").GetGuid();
                var yesCount = root.GetProperty("YesCount").GetInt32();
                var noCount = root.GetProperty("NoCount").GetInt32();

                VoteUpdated?.Invoke(proposalId, yesCount, noCount);
            }
            catch { }
        });

        _hubConnection.On<StopProposalDto>("ProposalResolved", proposal =>
        {
            ProposalResolved?.Invoke(proposal);
        });

        await _hubConnection.StartAsync();

        _currentTripId = tripId;
        await _hubConnection.InvokeAsync("JoinTrip", tripId);
    }

    public async Task SendLocationAsync(Guid tripId, double latitude, double longitude)
    {
        if (_hubConnection is not { State: HubConnectionState.Connected })
            return;

        await _hubConnection.InvokeAsync("SendLocationUpdate", tripId, latitude, longitude);
    }

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
