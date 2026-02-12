using MediatR;
using SyncTrip.Shared.DTOs.Chat;

namespace SyncTrip.Application.Chat.Queries;

/// <summary>
/// Query pour récupérer les messages d'un convoi avec pagination par curseur.
/// </summary>
public record GetConvoyMessagesQuery : IRequest<IList<MessageDto>>
{
    /// <summary>
    /// Identifiant du convoi.
    /// </summary>
    public Guid ConvoyId { get; init; }

    /// <summary>
    /// Identifiant de l'utilisateur faisant la requête.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Nombre de messages à récupérer.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Date avant laquelle récupérer les messages (curseur de pagination).
    /// </summary>
    public DateTime? Before { get; init; }
}
