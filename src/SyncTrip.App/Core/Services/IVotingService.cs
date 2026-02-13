using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.App.Core.Services;

public interface IVotingService
{
    Task<Guid?> ProposeStopAsync(Guid convoyId, Guid tripId, ProposeStopRequest request);
    Task<StopProposalDto?> GetActiveProposalAsync(Guid convoyId, Guid tripId);
    Task<List<StopProposalDto>> GetProposalHistoryAsync(Guid convoyId, Guid tripId);
    Task<bool> CastVoteAsync(Guid convoyId, Guid tripId, Guid proposalId, CastVoteRequest request);
}
