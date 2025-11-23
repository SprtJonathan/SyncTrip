using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Application.Auth.Commands;

/// <summary>
/// Handler pour le command SendMagicLink.
/// Implémente le principe "Blind Send" : retourne toujours un succès même si l'email n'existe pas.
/// </summary>
public class SendMagicLinkCommandHandler : IRequestHandler<SendMagicLinkCommand>
{
    private readonly IMagicLinkTokenRepository _tokenRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<SendMagicLinkCommandHandler> _logger;

    public SendMagicLinkCommandHandler(
        IMagicLinkTokenRepository tokenRepository,
        IEmailService emailService,
        ILogger<SendMagicLinkCommandHandler> logger)
    {
        _tokenRepository = tokenRepository;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Traite la commande d'envoi d'un Magic Link.
    /// Génère un token, le stocke et envoie l'email (blind send).
    /// </summary>
    public async Task Handle(SendMagicLinkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Normaliser l'email
            var email = request.Email.ToLowerInvariant().Trim();

            // Générer le token Magic Link
            var (tokenEntity, plainToken) = MagicLinkToken.Generate(email);

            // Stocker le token dans la base de données
            await _tokenRepository.CreateAsync(tokenEntity, cancellationToken);

            // Envoyer l'email avec le lien
            await _emailService.SendMagicLinkAsync(email, plainToken, cancellationToken);

            _logger.LogInformation("Magic Link généré et envoyé pour l'email {Email}", email);
        }
        catch (Exception ex)
        {
            // En mode "Blind Send", on ne propage pas l'erreur
            // On log simplement pour le monitoring
            _logger.LogError(ex, "Erreur lors de l'envoi du Magic Link pour {Email}", request.Email);
        }
    }
}
