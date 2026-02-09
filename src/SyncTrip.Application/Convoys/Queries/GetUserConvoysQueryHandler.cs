using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Convoys;

namespace SyncTrip.Application.Convoys.Queries;

/// <summary>
/// Handler pour récupérer les convois d'un utilisateur.
/// </summary>
public class GetUserConvoysQueryHandler : IRequestHandler<GetUserConvoysQuery, IList<ConvoyDto>>
{
    private readonly IConvoyRepository _convoyRepository;
    private readonly ILogger<GetUserConvoysQueryHandler> _logger;

    public GetUserConvoysQueryHandler(
        IConvoyRepository convoyRepository,
        ILogger<GetUserConvoysQueryHandler> logger)
    {
        _convoyRepository = convoyRepository;
        _logger = logger;
    }

    public async Task<IList<ConvoyDto>> Handle(GetUserConvoysQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Récupération des convois de l'utilisateur {UserId}", request.UserId);

        var convoys = await _convoyRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return convoys.Select(c =>
        {
            var leader = c.Members.FirstOrDefault(m => m.UserId == c.LeaderUserId);
            return new ConvoyDto
            {
                Id = c.Id,
                JoinCode = c.JoinCode,
                LeaderUsername = leader?.User?.Username ?? string.Empty,
                IsPrivate = c.IsPrivate,
                MemberCount = c.Members.Count,
                CreatedAt = c.CreatedAt
            };
        }).ToList();
    }
}
