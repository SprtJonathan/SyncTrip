using SyncTrip.Shared.DTOs.Chat;

namespace SyncTrip.App.Core.Services;

public interface IChatService
{
    Task<Guid?> SendMessageAsync(Guid convoyId, SendMessageRequest request);
    Task<List<MessageDto>> GetMessagesAsync(Guid convoyId, int pageSize = 50, DateTime? before = null);
}
