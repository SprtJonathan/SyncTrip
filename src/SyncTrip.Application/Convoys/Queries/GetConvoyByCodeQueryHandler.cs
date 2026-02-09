using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Convoys;

namespace SyncTrip.Application.Convoys.Queries;

/// <summary>
/// Handler pour récupérer les détails d'un convoi par code.
/// </summary>
public class GetConvoyByCodeQueryHandler : IRequestHandler<GetConvoyByCodeQuery, ConvoyDetailsDto?>
{
    private readonly IConvoyRepository _convoyRepository;
    private readonly ILogger<GetConvoyByCodeQueryHandler> _logger;

    public GetConvoyByCodeQueryHandler(
        IConvoyRepository convoyRepository,
        ILogger<GetConvoyByCodeQueryHandler> logger)
    {
        _convoyRepository = convoyRepository;
        _logger = logger;
    }

    public async Task<ConvoyDetailsDto?> Handle(GetConvoyByCodeQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recherche du convoi avec le code {JoinCode}", request.JoinCode);

        var convoy = await _convoyRepository.GetByJoinCodeAsync(request.JoinCode, cancellationToken);
        if (convoy == null)
            return null;

        var leader = convoy.Members.FirstOrDefault(m => m.UserId == convoy.LeaderUserId);

        return new ConvoyDetailsDto
        {
            Id = convoy.Id,
            JoinCode = convoy.JoinCode,
            LeaderUserId = convoy.LeaderUserId,
            LeaderUsername = leader?.User?.Username ?? string.Empty,
            IsPrivate = convoy.IsPrivate,
            CreatedAt = convoy.CreatedAt,
            Members = convoy.Members.Select(m => new ConvoyMemberDto
            {
                UserId = m.UserId,
                Username = m.User?.Username ?? string.Empty,
                AvatarUrl = m.User?.AvatarUrl,
                VehicleBrand = m.Vehicle?.Brand?.Name ?? string.Empty,
                VehicleModel = m.Vehicle?.Model ?? string.Empty,
                VehicleColor = m.Vehicle?.Color,
                Role = (int)m.Role,
                JoinedAt = m.JoinedAt
            }).ToList()
        };
    }
}
