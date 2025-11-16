using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using SyncTrip.Api.Application.DTOs.Auth;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Interfaces;

namespace SyncTrip.Api.Infrastructure.Services;

/// <summary>
/// Service d'authentification passwordless avec magic links
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<UserDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Vérifier si l'email existe déjà
        if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException("Cet email est déjà utilisé");
        }

        // Créer l'utilisateur
        var user = _mapper.Map<User>(request);
        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Nouvel utilisateur enregistré: {Email}", user.Email);

        return _mapper.Map<UserDto>(user);
    }

    public async Task SendMagicLinkAsync(string email, CancellationToken cancellationToken = default)
    {
        // Récupérer l'utilisateur
        var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("Utilisateur non trouvé");
        }

        // Générer un token unique
        var token = GenerateSecureToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(15); // 15 minutes de validité

        // Créer le magic link token
        var magicLinkToken = new MagicLinkToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = expiresAt
        };

        await _unitOfWork.MagicLinkTokens.AddAsync(magicLinkToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Envoyer l'email avec le magic link
        // Pour l'instant on log juste le token (à remplacer par un vrai service d'email)
        _logger.LogInformation("Magic link généré pour {Email}: {Token}", email, token);

        // Dans un vrai système, on enverrait un email ici avec le lien
        // await _emailService.SendMagicLinkAsync(email, token);
    }

    public async Task<AuthResponse> VerifyMagicLinkAsync(string token, CancellationToken cancellationToken = default)
    {
        // Récupérer le token valide
        var magicLinkToken = await _unitOfWork.MagicLinkTokens.GetValidTokenAsync(token, cancellationToken);
        if (magicLinkToken == null)
        {
            throw new UnauthorizedAccessException("Token invalide ou expiré");
        }

        // Marquer le token comme utilisé
        magicLinkToken.IsUsed = true;
        magicLinkToken.UsedAt = DateTime.UtcNow;
        _unitOfWork.MagicLinkTokens.Update(magicLinkToken);

        // Mettre à jour la dernière connexion
        var user = magicLinkToken.User;
        user.LastLoginAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Générer les tokens JWT
        var accessToken = GenerateJwtToken(user.Id, user.Email);
        var refreshToken = GenerateRefreshToken();

        _logger.LogInformation("Utilisateur authentifié: {Email}", user.Email);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // TODO: Implémenter la validation du refresh token
        // Pour l'instant, retourner une exception
        throw new NotImplementedException("Refresh token non implémenté");
    }

    public string GenerateJwtToken(Guid userId, string email)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"] ?? "SyncTripApi";
        var audience = jwtSettings["Audience"] ?? "SyncTripApp";

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
}
