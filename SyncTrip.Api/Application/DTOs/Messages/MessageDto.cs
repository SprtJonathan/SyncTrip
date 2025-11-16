using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Application.DTOs.Messages;

/// <summary>
/// DTO Message
/// </summary>
public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ConvoyId { get; set; }
    public Guid? UserId { get; set; }
    public string? UserDisplayName { get; set; }
    public MessageType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
