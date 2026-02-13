using SyncTrip.Shared.DTOs.Chat;

namespace SyncTrip.App.Core.Services;

public class ChatService : IChatService
{
    private readonly IApiService _apiService;

    public ChatService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<Guid?> SendMessageAsync(Guid convoyId, SendMessageRequest request)
    {
        var response = await _apiService.PostAsync<SendMessageRequest, MessageResponse>(
            $"api/convoys/{convoyId}/messages", request);
        return response?.MessageId;
    }

    public async Task<List<MessageDto>> GetMessagesAsync(Guid convoyId, int pageSize = 50, DateTime? before = null)
    {
        var url = $"api/convoys/{convoyId}/messages?pageSize={pageSize}";
        if (before.HasValue)
            url += $"&before={before.Value:O}";

        return await _apiService.GetAsync<List<MessageDto>>(url) ?? new();
    }

    private class MessageResponse
    {
        public Guid MessageId { get; set; }
    }
}
