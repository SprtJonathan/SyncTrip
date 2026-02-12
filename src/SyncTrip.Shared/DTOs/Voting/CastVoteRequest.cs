namespace SyncTrip.Shared.DTOs.Voting;

/// <summary>
/// Requête pour voter sur une proposition d'arrêt.
/// </summary>
public record CastVoteRequest
{
    /// <summary>
    /// True pour voter OUI, False pour voter NON.
    /// </summary>
    public bool IsYes { get; init; }
}
