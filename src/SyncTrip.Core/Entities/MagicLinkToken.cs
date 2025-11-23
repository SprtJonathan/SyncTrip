using System.Security.Cryptography;
using System.Text;

namespace SyncTrip.Core.Entities;

/// <summary>
/// Représente un token Magic Link pour l'authentification sans mot de passe.
/// </summary>
public class MagicLinkToken
{
    /// <summary>
    /// Identifiant unique du token.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Adresse email associée au token.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Token hashé (SHA256).
    /// </summary>
    public string Token { get; private set; } = string.Empty;

    /// <summary>
    /// Date d'expiration du token.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Date d'utilisation du token (null si non utilisé).
    /// </summary>
    public DateTime? UsedAt { get; private set; }

    /// <summary>
    /// Date de création du token.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Durée d'expiration par défaut en minutes.
    /// </summary>
    private const int DefaultExpirationMinutes = 10;

    // Constructeur privé pour EF Core
    private MagicLinkToken()
    {
    }

    /// <summary>
    /// Factory method pour générer un nouveau token Magic Link.
    /// </summary>
    /// <param name="email">Adresse email pour laquelle générer le token.</param>
    /// <param name="expirationMinutes">Durée de validité en minutes (par défaut 10).</param>
    /// <returns>Tuple contenant l'entité MagicLinkToken et le token en clair.</returns>
    public static (MagicLinkToken entity, string plainToken) Generate(
        string email,
        int expirationMinutes = DefaultExpirationMinutes)
    {
        // Générer un token aléatoire sécurisé
        string plainToken = GenerateSecureToken();

        // Hasher le token pour le stockage
        string hashedToken = HashToken(plainToken);

        var now = DateTime.UtcNow;

        var entity = new MagicLinkToken
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            Token = hashedToken,
            ExpiresAt = now.AddMinutes(expirationMinutes),
            CreatedAt = now
        };

        return (entity, plainToken);
    }

    /// <summary>
    /// Marque le token comme utilisé.
    /// </summary>
    public void MarkAsUsed()
    {
        UsedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Vérifie si le token est valide (non expiré et non utilisé).
    /// </summary>
    /// <returns>True si le token est valide, False sinon.</returns>
    public bool IsValid()
    {
        var now = DateTime.UtcNow;

        // Le token est valide s'il n'est pas expiré et n'a pas été utilisé
        return now <= ExpiresAt && UsedAt == null;
    }

    /// <summary>
    /// Vérifie si un token en clair correspond au token hashé.
    /// </summary>
    /// <param name="plainToken">Token en clair à vérifier.</param>
    /// <returns>True si le token correspond, False sinon.</returns>
    public bool VerifyToken(string plainToken)
    {
        string hashedToken = HashToken(plainToken);
        return Token == hashedToken;
    }

    /// <summary>
    /// Génère un token aléatoire sécurisé.
    /// </summary>
    /// <returns>Token de 32 caractères.</returns>
    private static string GenerateSecureToken()
    {
        // Générer 32 bytes aléatoires
        byte[] randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        // Convertir en base64 URL-safe
        return Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    /// <summary>
    /// Hash un token avec SHA256.
    /// </summary>
    /// <param name="token">Token en clair.</param>
    /// <returns>Token hashé en hexadécimal.</returns>
    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
        byte[] hashBytes = sha256.ComputeHash(tokenBytes);

        // Convertir en hexadécimal
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
