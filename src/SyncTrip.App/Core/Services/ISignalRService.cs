namespace SyncTrip.App.Core.Services;

public interface ISignalRService
{
    bool IsConnected { get; }
    event Action<string, double, double, DateTime>? LocationReceived;
    Task ConnectAsync(Guid tripId);
    Task SendLocationAsync(Guid tripId, double latitude, double longitude);
    Task DisconnectAsync();
}
