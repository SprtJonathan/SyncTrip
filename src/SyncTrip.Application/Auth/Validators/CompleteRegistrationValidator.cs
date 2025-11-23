using FluentValidation;
using SyncTrip.Application.Auth.Commands;

namespace SyncTrip.Application.Auth.Validators;

/// <summary>
/// Validator pour CompleteRegistrationCommand.
/// Valide les règles métier : username obligatoire, email valide, âge > 14 ans.
/// </summary>
public class CompleteRegistrationValidator : AbstractValidator<CompleteRegistrationCommand>
{
    /// <summary>
    /// Âge minimum requis pour utiliser l'application.
    /// </summary>
    private const int MinimumAge = 14;

    public CompleteRegistrationValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'adresse email est obligatoire")
            .EmailAddress().WithMessage("L'adresse email n'est pas valide")
            .MaximumLength(255).WithMessage("L'email ne peut pas dépasser 255 caractères");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Le pseudo est obligatoire")
            .MaximumLength(50).WithMessage("Le pseudo ne peut pas dépasser 50 caractères")
            .Matches(@"^[a-zA-Z0-9_-]+$")
            .WithMessage("Le pseudo ne peut contenir que des lettres, chiffres, tirets et underscores");

        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("Le prénom ne peut pas dépasser 100 caractères")
            .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Le nom ne peut pas dépasser 100 caractères")
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("La date de naissance est obligatoire")
            .Must(BeOlderThan14)
            .WithMessage($"Vous devez avoir plus de {MinimumAge} ans pour utiliser cette application");
    }

    /// <summary>
    /// Vérifie que l'utilisateur a plus de 14 ans.
    /// </summary>
    private static bool BeOlderThan14(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        int age = today.Year - birthDate.Year;

        // Ajuster si l'anniversaire n'est pas encore passé cette année
        if (birthDate > today.AddYears(-age))
            age--;

        return age > MinimumAge;
    }
}
