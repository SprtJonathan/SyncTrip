using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Application.DTOs.Convoys;

/// <summary>
/// DTO Convoi
/// </summary>
public class ConvoyDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Name { get; set; }
    public ConvoyStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ParticipantsCount { get; set; }
    public List<ConvoyParticipantDto> Participants { get; set; } = new();
}
