using SyncTrip.Shared.DTOs.Chat;

namespace SyncTrip.App.Core.Services;

public interface IConvoySignalRService
{
    bool IsConnected { get; }
    event Action<MessageDto>? MessageReceived;
    Task ConnectAsync(Guid convoyId);
    Task DisconnectAsync();
}
