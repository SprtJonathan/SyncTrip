using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Auth.Commands;

/// <summary>
/// Handler pour compléter l'inscription d'un nouvel utilisateur.
/// </summary>
public class CompleteRegistrationCommandHandler : IRequestHandler<CompleteRegistrationCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly ILogger<CompleteRegistrationCommandHandler> _logger;

    public CompleteRegistrationCommandHandler(
        IUserRepository userRepository,
        IAuthService authService,
        ILogger<CompleteRegistrationCommandHandler> logger)
    {
        _userRepository = userRepository;
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Crée un nouvel utilisateur et retourne un JWT.
    /// </summary>
    /// <returns>Token JWT pour l'utilisateur créé.</returns>
    public async Task<string> Handle(CompleteRegistrationCommand request, CancellationToken cancellationToken)
    {
        // Normaliser l'email
        var email = request.Email.ToLowerInvariant().Trim();

        // Vérifier si l'utilisateur existe déjà
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existingUser != null)
        {
            _logger.LogWarning("Tentative de création d'un utilisateur déjà existant : {Email}", email);
            throw new InvalidOperationException("Un utilisateur avec cet email existe déjà");
        }

        // Créer l'utilisateur (la validation de l'âge se fait dans le factory method)
        var user = User.Create(
            email,
            request.Username.Trim(),
            request.BirthDate,
            request.FirstName?.Trim(),
            request.LastName?.Trim()
        );

        // Sauvegarder en base de données
        await _userRepository.AddAsync(user, cancellationToken);

        _logger.LogInformation("Nouvel utilisateur créé : {Email}, {Username}", user.Email, user.Username);

        // Générer un JWT pour l'utilisateur
        var jwtToken = _authService.GenerateJwtToken(user);

        return jwtToken;
    }
}
