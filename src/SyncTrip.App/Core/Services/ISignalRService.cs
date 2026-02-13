using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.App.Core.Services;

public interface ISignalRService
{
    bool IsConnected { get; }
    event Action<string, double, double, DateTime>? LocationReceived;
    event Action<StopProposalDto>? StopProposed;
    event Action<Guid, int, int>? VoteUpdated;
    event Action<StopProposalDto>? ProposalResolved;
    Task ConnectAsync(Guid tripId);
    Task SendLocationAsync(Guid tripId, double latitude, double longitude);
    Task DisconnectAsync();
}
