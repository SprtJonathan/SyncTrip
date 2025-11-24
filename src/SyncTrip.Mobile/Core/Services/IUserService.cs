using SyncTrip.Shared.DTOs.Users;

namespace SyncTrip.Mobile.Core.Services;

/// <summary>
/// Service de gestion du profil utilisateur.
/// Permet de récupérer et mettre à jour les informations du profil.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Récupère le profil complet de l'utilisateur connecté.
    /// </summary>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>Profil utilisateur ou null en cas d'échec.</returns>
    Task<UserProfileDto?> GetProfileAsync(CancellationToken ct = default);

    /// <summary>
    /// Met à jour le profil de l'utilisateur connecté.
    /// </summary>
    /// <param name="request">Données de mise à jour du profil.</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>True si la mise à jour a réussi, False sinon.</returns>
    Task<bool> UpdateProfileAsync(UpdateUserProfileRequest request, CancellationToken ct = default);
}
