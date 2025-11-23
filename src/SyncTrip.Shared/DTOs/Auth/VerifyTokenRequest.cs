namespace SyncTrip.Shared.DTOs.Auth;

/// <summary>
/// Requête pour vérifier un token Magic Link.
/// </summary>
/// <param name="Token">Token Magic Link à vérifier.</param>
public record VerifyTokenRequest(string Token);
