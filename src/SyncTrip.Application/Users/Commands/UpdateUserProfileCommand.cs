using MediatR;

namespace SyncTrip.Application.Users.Commands;

/// <summary>
/// Command pour mettre à jour le profil d'un utilisateur.
/// </summary>
public record UpdateUserProfileCommand : IRequest
{
    /// <summary>
    /// Identifiant de l'utilisateur à mettre à jour.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Nouveau nom d'utilisateur (pseudo).
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Nouveau prénom.
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// Nouveau nom de famille.
    /// </summary>
    public string? LastName { get; init; }

    /// <summary>
    /// Nouvelle date de naissance (doit avoir plus de 14 ans).
    /// </summary>
    public DateOnly? BirthDate { get; init; }

    /// <summary>
    /// Nouvelle URL d'avatar.
    /// </summary>
    public string? AvatarUrl { get; init; }

    /// <summary>
    /// Liste des types de permis (remplace la liste existante).
    /// </summary>
    public List<int>? LicenseTypes { get; init; }
}
