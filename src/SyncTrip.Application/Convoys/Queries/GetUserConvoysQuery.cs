using MediatR;
using SyncTrip.Shared.DTOs.Convoys;

namespace SyncTrip.Application.Convoys.Queries;

/// <summary>
/// Query pour récupérer tous les convois d'un utilisateur.
/// </summary>
public record GetUserConvoysQuery : IRequest<IList<ConvoyDto>>
{
    /// <summary>
    /// Identifiant de l'utilisateur.
    /// </summary>
    public Guid UserId { get; init; }

    public GetUserConvoysQuery(Guid userId)
    {
        UserId = userId;
    }
}
