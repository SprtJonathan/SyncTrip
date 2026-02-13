using Microsoft.AspNetCore.SignalR.Client;
using SyncTrip.Shared.DTOs.Chat;

namespace SyncTrip.App.Core.Services;

public class ConvoySignalRService : IConvoySignalRService
{
    private readonly IAuthenticationService _authService;
    private HubConnection? _hubConnection;
    private Guid _currentConvoyId;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public event Action<MessageDto>? MessageReceived;

    public ConvoySignalRService(IAuthenticationService authService)
    {
        _authService = authService;
    }

    public async Task ConnectAsync(Guid convoyId)
    {
        if (IsConnected)
            await DisconnectAsync();

        var token = await _authService.GetTokenAsync();

        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/hubs/convoy", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<MessageDto>("ReceiveMessage", message =>
        {
            MessageReceived?.Invoke(message);
        });

        await _hubConnection.StartAsync();

        _currentConvoyId = convoyId;
        await _hubConnection.InvokeAsync("JoinConvoy", convoyId);
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection is null)
            return;

        try
        {
            if (_hubConnection.State == HubConnectionState.Connected)
                await _hubConnection.InvokeAsync("LeaveConvoy", _currentConvoyId);

            await _hubConnection.StopAsync();
        }
        finally
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }
}
