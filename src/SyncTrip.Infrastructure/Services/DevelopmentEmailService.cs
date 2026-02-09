using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Infrastructure.Services;

/// <summary>
/// Service d'email pour le développement local.
/// Log le magic link dans la console au lieu d'envoyer un vrai email.
/// </summary>
public class DevelopmentEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DevelopmentEmailService> _logger;

    public DevelopmentEmailService(IConfiguration configuration, ILogger<DevelopmentEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task SendMagicLinkAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var appBaseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:5001";
        var magicLinkUrl = $"{appBaseUrl}/auth/verify?token={token}";

        _logger.LogInformation(
            "===== MAGIC LINK (DEV) =====\n" +
            "  Email: {Email}\n" +
            "  Token: {Token}\n" +
            "  URL:   {Url}\n" +
            "============================",
            email, token, magicLinkUrl);

        return Task.CompletedTask;
    }
}
