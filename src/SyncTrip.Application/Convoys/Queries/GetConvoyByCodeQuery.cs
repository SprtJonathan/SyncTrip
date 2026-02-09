using MediatR;
using SyncTrip.Shared.DTOs.Convoys;

namespace SyncTrip.Application.Convoys.Queries;

/// <summary>
/// Query pour récupérer les détails d'un convoi par son code d'accès.
/// </summary>
public record GetConvoyByCodeQuery : IRequest<ConvoyDetailsDto?>
{
    /// <summary>
    /// Code d'accès du convoi.
    /// </summary>
    public string JoinCode { get; init; } = string.Empty;

    public GetConvoyByCodeQuery(string joinCode)
    {
        JoinCode = joinCode;
    }
}
