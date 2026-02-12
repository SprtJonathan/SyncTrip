namespace SyncTrip.Shared.DTOs.Voting;

/// <summary>
/// Requête pour proposer un arrêt.
/// </summary>
public record ProposeStopRequest
{
    /// <summary>
    /// Type d'arrêt (1=Fuel, 2=Break, 3=Food, 4=Photo).
    /// </summary>
    public int StopType { get; init; }

    /// <summary>
    /// Latitude de l'arrêt proposé.
    /// </summary>
    public double Latitude { get; init; }

    /// <summary>
    /// Longitude de l'arrêt proposé.
    /// </summary>
    public double Longitude { get; init; }

    /// <summary>
    /// Nom du lieu de l'arrêt proposé.
    /// </summary>
    public string LocationName { get; init; } = string.Empty;
}
