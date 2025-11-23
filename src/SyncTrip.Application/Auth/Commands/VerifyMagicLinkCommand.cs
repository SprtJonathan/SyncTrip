using MediatR;
using SyncTrip.Shared.DTOs.Auth;

namespace SyncTrip.Application.Auth.Commands;

/// <summary>
/// Command pour vérifier un token Magic Link.
/// </summary>
/// <param name="Token">Token Magic Link à vérifier.</param>
public record VerifyMagicLinkCommand(string Token) : IRequest<VerifyTokenResponse>;
