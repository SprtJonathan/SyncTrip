namespace SyncTrip.Shared.DTOs.Convoys;

/// <summary>
/// Requête pour rejoindre un convoi existant.
/// </summary>
public record JoinConvoyRequest
{
    /// <summary>
    /// Identifiant du véhicule utilisé pour ce convoi.
    /// </summary>
    public Guid VehicleId { get; init; }
}
