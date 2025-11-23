namespace SyncTrip.Shared.DTOs.Auth;

/// <summary>
/// Réponse à la vérification d'un token Magic Link.
/// </summary>
public class VerifyTokenResponse
{
    /// <summary>
    /// Indique si la vérification a réussi.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Token JWT si l'utilisateur existe, null sinon.
    /// </summary>
    public string? JwtToken { get; init; }

    /// <summary>
    /// Indique si l'utilisateur doit compléter son inscription.
    /// </summary>
    public bool RequiresRegistration { get; init; }

    /// <summary>
    /// Message explicatif.
    /// </summary>
    public string Message { get; init; } = string.Empty;
}
