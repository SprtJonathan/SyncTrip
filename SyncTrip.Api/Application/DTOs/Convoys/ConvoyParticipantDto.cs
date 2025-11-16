using SyncTrip.Api.Application.DTOs.Auth;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Application.DTOs.Convoys;

/// <summary>
/// DTO Participant de convoi
/// </summary>
public class ConvoyParticipantDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserDisplayName { get; set; } = string.Empty;
    public ConvoyRole Role { get; set; }
    public string? VehicleName { get; set; }
    public DateTime JoinedAt { get; set; }
}
