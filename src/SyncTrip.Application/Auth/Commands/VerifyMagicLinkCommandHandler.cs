using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Auth;
using System.Security.Cryptography;
using System.Text;

namespace SyncTrip.Application.Auth.Commands;

/// <summary>
/// Handler pour vérifier un token Magic Link.
/// Si l'email existe : génère un JWT normal.
/// Si l'email n'existe pas : génère un JWT avec scope "registration".
/// </summary>
public class VerifyMagicLinkCommandHandler : IRequestHandler<VerifyMagicLinkCommand, VerifyTokenResponse>
{
    private readonly IMagicLinkTokenRepository _tokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly ILogger<VerifyMagicLinkCommandHandler> _logger;

    public VerifyMagicLinkCommandHandler(
        IMagicLinkTokenRepository tokenRepository,
        IUserRepository userRepository,
        IAuthService authService,
        ILogger<VerifyMagicLinkCommandHandler> logger)
    {
        _tokenRepository = tokenRepository;
        _userRepository = userRepository;
        _authService = authService;
        _logger = logger;
    }

    public async Task<VerifyTokenResponse> Handle(VerifyMagicLinkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Hasher le token pour le chercher en DB
            string hashedToken = HashToken(request.Token);

            // Récupérer le token depuis la base de données
            var tokenEntity = await _tokenRepository.GetByTokenAsync(hashedToken, cancellationToken);

            if (tokenEntity == null)
            {
                _logger.LogWarning("Token Magic Link invalide ou non trouvé");
                return new VerifyTokenResponse
                {
                    Success = false,
                    Message = "Token invalide ou expiré"
                };
            }

            // Vérifier que le token est valide (non expiré, non utilisé)
            if (!tokenEntity.IsValid())
            {
                _logger.LogWarning("Token Magic Link expiré ou déjà utilisé pour {Email}", tokenEntity.Email);
                return new VerifyTokenResponse
                {
                    Success = false,
                    Message = "Token expiré ou déjà utilisé"
                };
            }

            // Vérifier le token
            if (!tokenEntity.VerifyToken(request.Token))
            {
                _logger.LogWarning("Token Magic Link ne correspond pas au hash stocké");
                return new VerifyTokenResponse
                {
                    Success = false,
                    Message = "Token invalide"
                };
            }

            // Marquer le token comme utilisé
            tokenEntity.MarkAsUsed();
            await _tokenRepository.MarkAsUsedAsync(tokenEntity, cancellationToken);

            // Vérifier si l'utilisateur existe déjà
            var user = await _userRepository.GetByEmailAsync(tokenEntity.Email, cancellationToken);

            if (user != null)
            {
                // L'utilisateur existe : générer un JWT normal
                var jwtToken = _authService.GenerateJwtToken(user);

                _logger.LogInformation("Utilisateur {Email} authentifié avec succès", user.Email);

                return new VerifyTokenResponse
                {
                    Success = true,
                    JwtToken = jwtToken,
                    RequiresRegistration = false,
                    Message = "Authentification réussie"
                };
            }
            else
            {
                // L'utilisateur n'existe pas : générer un JWT avec scope "registration"
                // On crée un user temporaire juste pour la génération du JWT
                var tempUser = Core.Entities.User.Create(
                    tokenEntity.Email,
                    "temp", // Username temporaire
                    DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-15)) // Date de naissance valide temporaire
                );

                var claims = new Dictionary<string, string>
                {
                    { "scope", "registration" },
                    { "email", tokenEntity.Email }
                };

                var jwtToken = _authService.GenerateJwtToken(tempUser, claims);

                _logger.LogInformation("Token de registration généré pour {Email}", tokenEntity.Email);

                return new VerifyTokenResponse
                {
                    Success = true,
                    JwtToken = jwtToken,
                    RequiresRegistration = true,
                    Message = "Veuillez compléter votre inscription"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification du token Magic Link");
            return new VerifyTokenResponse
            {
                Success = false,
                Message = "Une erreur est survenue lors de la vérification"
            };
        }
    }

    /// <summary>
    /// Hash un token avec SHA256 (doit correspondre à la méthode de MagicLinkToken).
    /// </summary>
    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
        byte[] hashBytes = sha256.ComputeHash(tokenBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
