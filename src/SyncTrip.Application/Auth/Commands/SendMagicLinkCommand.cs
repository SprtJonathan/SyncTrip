using MediatR;

namespace SyncTrip.Application.Auth.Commands;

/// <summary>
/// Command pour envoyer un lien Magic Link par email.
/// Utilise le principe "Blind Send" : aucune information n'est divulguée sur l'existence du compte.
/// </summary>
/// <param name="Email">Adresse email du destinataire.</param>
public record SendMagicLinkCommand(string Email) : IRequest;
