using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.App.Core.Services;

public class VotingService : IVotingService
{
    private readonly IApiService _apiService;

    public VotingService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<Guid?> ProposeStopAsync(Guid convoyId, Guid tripId, ProposeStopRequest request)
    {
        var response = await _apiService.PostAsync<ProposeStopRequest, ProposalResponse>(
            $"api/convoys/{convoyId}/trips/{tripId}/proposals", request);
        return response?.ProposalId;
    }

    public async Task<StopProposalDto?> GetActiveProposalAsync(Guid convoyId, Guid tripId)
    {
        return await _apiService.GetAsync<StopProposalDto>(
            $"api/convoys/{convoyId}/trips/{tripId}/proposals/active");
    }

    public async Task<List<StopProposalDto>> GetProposalHistoryAsync(Guid convoyId, Guid tripId)
    {
        return await _apiService.GetAsync<List<StopProposalDto>>(
            $"api/convoys/{convoyId}/trips/{tripId}/proposals") ?? new();
    }

    public async Task<bool> CastVoteAsync(Guid convoyId, Guid tripId, Guid proposalId, CastVoteRequest request)
    {
        var response = await _apiService.PostAsync<CastVoteRequest, object>(
            $"api/convoys/{convoyId}/trips/{tripId}/proposals/{proposalId}/vote", request);
        return response is not null;
    }

    private class ProposalResponse
    {
        public Guid ProposalId { get; set; }
    }
}
