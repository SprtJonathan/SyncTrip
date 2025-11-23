using MediatR;
using Microsoft.AspNetCore.Mvc;
using SyncTrip.Application.Auth.Commands;
using SyncTrip.Shared.DTOs.Auth;

namespace SyncTrip.API.Controllers;

/// <summary>
/// Contrôleur d'authentification avec Magic Link.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initialise une nouvelle instance du contrôleur d'authentification.
    /// </summary>
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Envoie un Magic Link par email (Blind Send).
    /// </summary>
    /// <param name="request">Requête contenant l'email.</param>
    /// <returns>Message de confirmation générique.</returns>
    /// <remarks>
    /// Ce endpoint ne divulgue jamais si l'email existe ou non dans la base de données.
    /// Le message de retour est toujours identique pour éviter l'énumération de comptes.
    /// </remarks>
    [HttpPost("magic-link")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SendMagicLink([FromBody] MagicLinkRequest request)
    {
        var command = new SendMagicLinkCommand(request.Email);
        await _mediator.Send(command);
        return Ok(new { Message = "Si un compte existe avec cet email, vous recevrez un lien de connexion." });
    }

    /// <summary>
    /// Vérifie le token Magic Link et retourne JWT si valide.
    /// </summary>
    /// <param name="request">Requête contenant le token à vérifier.</param>
    /// <returns>Réponse contenant le JWT ou indiquant qu'une inscription est requise.</returns>
    [HttpPost("verify")]
    [ProducesResponseType<VerifyTokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyToken([FromBody] VerifyTokenRequest request)
    {
        var command = new VerifyMagicLinkCommand(request.Token);
        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(new { result.Message });

        return Ok(result);
    }

    /// <summary>
    /// Complète l'inscription d'un nouvel utilisateur.
    /// </summary>
    /// <param name="request">Informations d'inscription de l'utilisateur.</param>
    /// <returns>JWT token pour l'utilisateur nouvellement créé.</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteRegistration([FromBody] CompleteRegistrationRequest request)
    {
        var command = new CompleteRegistrationCommand
        {
            Email = request.Email,
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate
        };

        var jwtToken = await _mediator.Send(command);

        return CreatedAtAction(nameof(CompleteRegistration), new { JwtToken = jwtToken });
    }
}
