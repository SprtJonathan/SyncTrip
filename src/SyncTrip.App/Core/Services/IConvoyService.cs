using SyncTrip.Shared.DTOs.Convoys;

namespace SyncTrip.App.Core.Services;

public interface IConvoyService
{
    Task<Guid?> CreateConvoyAsync(CreateConvoyRequest request, CancellationToken ct = default);
    Task<ConvoyDetailsDto?> GetConvoyByCodeAsync(string joinCode, CancellationToken ct = default);
    Task<List<ConvoyDto>> GetMyConvoysAsync(CancellationToken ct = default);
    Task<bool> JoinConvoyAsync(string joinCode, JoinConvoyRequest request, CancellationToken ct = default);
    Task<bool> LeaveConvoyAsync(string joinCode, CancellationToken ct = default);
    Task<bool> DissolveConvoyAsync(string joinCode, CancellationToken ct = default);
}
