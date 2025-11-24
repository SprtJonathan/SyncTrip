namespace SyncTrip.Core.Enums;

/// <summary>
/// Types de permis de conduire disponibles en France.
/// Les valeurs correspondent aux catégories officielles.
/// </summary>
public enum LicenseType
{
    /// <summary>
    /// Permis AM - Cyclomoteurs et quadricycles légers (14 ans minimum).
    /// </summary>
    AM = 1,

    /// <summary>
    /// Permis A1 - Motocyclettes légères jusqu'à 125 cm³ (16 ans minimum).
    /// </summary>
    A1 = 2,

    /// <summary>
    /// Permis A2 - Motocyclettes de puissance intermédiaire (18 ans minimum).
    /// </summary>
    A2 = 3,

    /// <summary>
    /// Permis A - Toutes les motocyclettes (20/24 ans minimum selon parcours).
    /// </summary>
    A = 4,

    /// <summary>
    /// Permis B - Véhicules légers (voitures) jusqu'à 3,5 tonnes (18 ans minimum).
    /// </summary>
    B = 5,

    /// <summary>
    /// Permis BE - Véhicule léger avec remorque > 750 kg.
    /// </summary>
    BE = 6,

    /// <summary>
    /// Permis C - Véhicules de transport de marchandises > 3,5 tonnes (21 ans minimum).
    /// </summary>
    C = 7,

    /// <summary>
    /// Permis CE - Véhicule lourd avec remorque > 750 kg.
    /// </summary>
    CE = 8,

    /// <summary>
    /// Permis D - Véhicules de transport de personnes > 9 places (24 ans minimum).
    /// </summary>
    D = 9,

    /// <summary>
    /// Permis DE - Bus avec remorque > 750 kg.
    /// </summary>
    DE = 10
}
