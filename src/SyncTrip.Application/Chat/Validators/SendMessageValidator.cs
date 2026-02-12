using FluentValidation;
using SyncTrip.Application.Chat.Commands;

namespace SyncTrip.Application.Chat.Validators;

/// <summary>
/// Validator pour SendMessageCommand.
/// </summary>
public class SendMessageValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.ConvoyId)
            .NotEmpty().WithMessage("L'identifiant du convoi est obligatoire.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("L'identifiant utilisateur est obligatoire.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Le contenu du message est obligatoire.")
            .MaximumLength(500).WithMessage("Le message ne peut pas dépasser 500 caractères.");
    }
}
