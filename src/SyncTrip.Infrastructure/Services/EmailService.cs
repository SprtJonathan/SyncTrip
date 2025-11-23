using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using SyncTrip.Core.Interfaces;

namespace SyncTrip.Infrastructure.Services;

/// <summary>
/// Service d'envoi d'emails via SMTP (MailKit).
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendMagicLinkAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpHost = emailSettings["SmtpHost"] ?? "localhost";
            var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
            var smtpUser = emailSettings["SmtpUser"] ?? "";
            var smtpPassword = emailSettings["SmtpPassword"] ?? "";
            var fromEmail = emailSettings["FromEmail"] ?? "noreply@synctrip.com";
            var fromName = emailSettings["FromName"] ?? "SyncTrip";

            // Construire l'URL du Magic Link
            var appBaseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:5001";
            var magicLinkUrl = $"{appBaseUrl}/auth/verify?token={token}";

            // Créer le message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Votre lien de connexion SyncTrip";

            // Corps de l'email (HTML)
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='color: white; margin: 0;'>SyncTrip</h1>
    </div>
    <div style='background: #ffffff; padding: 30px; border: 1px solid #e0e0e0; border-radius: 0 0 10px 10px;'>
        <h2 style='color: #667eea; margin-top: 0;'>Connexion à votre compte</h2>
        <p>Bonjour,</p>
        <p>Vous avez demandé à vous connecter à SyncTrip. Cliquez sur le bouton ci-dessous pour vous connecter :</p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{magicLinkUrl}'
               style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                      color: white;
                      padding: 15px 30px;
                      text-decoration: none;
                      border-radius: 5px;
                      display: inline-block;
                      font-weight: bold;'>
                Se connecter
            </a>
        </div>
        <p style='font-size: 12px; color: #666;'>
            Ce lien expirera dans 10 minutes et ne peut être utilisé qu'une seule fois.
        </p>
        <p style='font-size: 12px; color: #666;'>
            Si vous n'avez pas demandé cette connexion, vous pouvez ignorer cet email en toute sécurité.
        </p>
        <hr style='border: none; border-top: 1px solid #e0e0e0; margin: 30px 0;'>
        <p style='font-size: 12px; color: #999; text-align: center;'>
            SyncTrip - Synchronisez vos convois en toute simplicité
        </p>
    </div>
</body>
</html>",
                TextBody = $@"
SyncTrip - Connexion à votre compte

Bonjour,

Vous avez demandé à vous connecter à SyncTrip.

Cliquez sur ce lien pour vous connecter :
{magicLinkUrl}

Ce lien expirera dans 10 minutes et ne peut être utilisé qu'une seule fois.

Si vous n'avez pas demandé cette connexion, vous pouvez ignorer cet email en toute sécurité.

---
SyncTrip - Synchronisez vos convois en toute simplicité
"
            };

            message.Body = bodyBuilder.ToMessageBody();

            // Envoyer l'email
            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, cancellationToken);

            // Authentification si nécessaire
            if (!string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPassword))
            {
                await client.AuthenticateAsync(smtpUser, smtpPassword, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email Magic Link envoyé avec succès à {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi du Magic Link par email à {Email}", email);
            throw;
        }
    }
}
