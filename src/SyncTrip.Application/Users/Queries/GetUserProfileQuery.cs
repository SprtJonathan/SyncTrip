using MediatR;
using SyncTrip.Shared.DTOs.Users;

namespace SyncTrip.Application.Users.Queries;

/// <summary>
/// Query pour récupérer le profil complet d'un utilisateur.
/// </summary>
public record GetUserProfileQuery : IRequest<UserProfileDto>
{
    /// <summary>
    /// Identifiant de l'utilisateur dont on veut récupérer le profil.
    /// </summary>
    public Guid UserId { get; init; }

    public GetUserProfileQuery(Guid userId)
    {
        UserId = userId;
    }
}
