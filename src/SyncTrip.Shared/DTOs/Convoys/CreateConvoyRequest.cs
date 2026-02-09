namespace SyncTrip.Shared.DTOs.Convoys;

/// <summary>
/// Requête pour créer un nouveau convoi.
/// </summary>
public record CreateConvoyRequest
{
    /// <summary>
    /// Identifiant du véhicule utilisé par le leader.
    /// </summary>
    public Guid VehicleId { get; init; }

    /// <summary>
    /// Indique si le convoi est privé (validation par le leader pour rejoindre).
    /// </summary>
    public bool IsPrivate { get; init; }
}
