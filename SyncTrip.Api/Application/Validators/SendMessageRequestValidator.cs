using FluentValidation;
using SyncTrip.Api.Application.DTOs.Messages;

namespace SyncTrip.Api.Application.Validators;

/// <summary>
/// Validator pour l'envoi de message
/// </summary>
public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Le contenu est obligatoire")
            .MinimumLength(1).WithMessage("Le message ne peut pas être vide")
            .MaximumLength(2000).WithMessage("Le message ne peut pas dépasser 2000 caractères");
    }
}
