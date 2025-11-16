namespace SyncTrip.Api.Core.Enums;

/// <summary>
/// Préférences de calcul d'itinéraire
/// </summary>
public enum RoutePreference
{
    /// <summary>
    /// Route la plus rapide
    /// </summary>
    Fastest = 0,

    /// <summary>
    /// Route panoramique
    /// </summary>
    Scenic = 1,

    /// <summary>
    /// Route économique (consommation minimale)
    /// </summary>
    Economical = 2,

    /// <summary>
    /// Route la plus courte (distance minimale)
    /// </summary>
    Shortest = 3
}
