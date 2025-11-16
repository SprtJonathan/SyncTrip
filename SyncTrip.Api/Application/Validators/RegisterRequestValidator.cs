using FluentValidation;
using SyncTrip.Api.Application.DTOs.Auth;

namespace SyncTrip.Api.Application.Validators;

/// <summary>
/// Validator pour la requête d'inscription
/// </summary>
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est obligatoire")
            .EmailAddress().WithMessage("Format d'email invalide")
            .MaximumLength(255).WithMessage("L'email ne peut pas dépasser 255 caractères");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Le nom d'affichage est obligatoire")
            .MinimumLength(2).WithMessage("Le nom doit contenir au moins 2 caractères")
            .MaximumLength(100).WithMessage("Le nom ne peut pas dépasser 100 caractères");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Format de téléphone invalide (format international attendu)");
    }
}
