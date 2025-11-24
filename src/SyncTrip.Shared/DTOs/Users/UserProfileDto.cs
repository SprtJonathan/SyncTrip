namespace SyncTrip.Shared.DTOs.Users;

/// <summary>
/// DTO représentant le profil complet d'un utilisateur.
/// </summary>
public class UserProfileDto
{
    /// <summary>
    /// Identifiant unique de l'utilisateur.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Adresse email de l'utilisateur.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Nom d'utilisateur (pseudo).
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
    /// Date de naissance de l'utilisateur.
    /// </summary>
    public DateOnly BirthDate { get; init; }

    /// <summary>
    /// URL de l'avatar de l'utilisateur (facultatif).
    /// </summary>
    public string? AvatarUrl { get; init; }

    /// <summary>
    /// Âge calculé de l'utilisateur.
    /// </summary>
    public int Age { get; init; }

    /// <summary>
    /// Liste des permis de conduire de l'utilisateur.
    /// </summary>
    public List<int> LicenseTypes { get; init; } = new();

    /// <summary>
    /// Date de création du compte.
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
