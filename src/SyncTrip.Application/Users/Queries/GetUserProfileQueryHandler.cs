using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Users;

namespace SyncTrip.Application.Users.Queries;

/// <summary>
/// Handler pour récupérer le profil complet d'un utilisateur.
/// </summary>
public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserProfileQueryHandler> _logger;

    public GetUserProfileQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUserProfileQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Récupère le profil d'un utilisateur avec ses permis.
    /// </summary>
    /// <param name="request">Query contenant l'ID utilisateur.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>DTO du profil utilisateur.</returns>
    /// <exception cref="KeyNotFoundException">Si l'utilisateur n'existe pas.</exception>
    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithLicensesAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Utilisateur introuvable : {UserId}", request.UserId);
            throw new KeyNotFoundException($"Utilisateur avec l'ID {request.UserId} introuvable");
        }

        var licenseTypes = user.Licenses
            .Select(l => (int)l.LicenseType)
            .ToList();

        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            BirthDate = user.BirthDate,
            AvatarUrl = user.AvatarUrl,
            Age = user.CalculateAge(),
            LicenseTypes = licenseTypes,
            CreatedAt = user.CreatedAt
        };
    }
}
