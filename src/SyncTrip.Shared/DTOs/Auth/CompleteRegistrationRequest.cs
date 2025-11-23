namespace SyncTrip.Shared.DTOs.Auth;

/// <summary>
/// Requête pour compléter l'inscription d'un utilisateur.
/// </summary>
public class CompleteRegistrationRequest
{
    /// <summary>
    /// Adresse email de l'utilisateur.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Nom d'utilisateur (pseudo) - obligatoire.
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// Prénom de l'utilisateur (facultatif).
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// Nom de famille de l'utilisateur (facultatif).
    /// </summary>
    public string? LastName { get; init; }

    /// <summary>
    /// Date de naissance de l'utilisateur (obligatoire).
    /// Doit avoir plus de 14 ans.
    /// </summary>
    public DateOnly BirthDate { get; init; }
}
