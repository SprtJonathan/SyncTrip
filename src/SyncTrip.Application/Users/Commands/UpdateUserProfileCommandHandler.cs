using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Users.Commands;

/// <summary>
/// Handler pour mettre à jour le profil d'un utilisateur.
/// </summary>
public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

    public UpdateUserProfileCommandHandler(
        IUserRepository userRepository,
        ILogger<UpdateUserProfileCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Met à jour le profil de l'utilisateur et ses permis.
    /// </summary>
    /// <param name="request">Command contenant les nouvelles informations.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <exception cref="KeyNotFoundException">Si l'utilisateur n'existe pas.</exception>
    public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithLicensesAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Utilisateur introuvable lors de la mise à jour : {UserId}", request.UserId);
            throw new KeyNotFoundException($"Utilisateur avec l'ID {request.UserId} introuvable");
        }

        // Mettre à jour les informations du profil
        user.UpdateProfile(
            request.Username,
            request.FirstName,
            request.LastName,
            request.AvatarUrl
        );

        // Mettre à jour la date de naissance si fournie
        if (request.BirthDate.HasValue)
        {
            user.SetBirthDate(request.BirthDate.Value);
        }

        // Mettre à jour les permis si fournis
        if (request.LicenseTypes != null)
        {
            await _userRepository.UpdateUserLicensesAsync(
                user.Id,
                request.LicenseTypes.Cast<LicenseType>().ToList(),
                cancellationToken
            );
        }

        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Profil mis à jour pour l'utilisateur {UserId}", user.Id);
    }
}
