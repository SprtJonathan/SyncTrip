using FluentValidation;
using SyncTrip.Application.Users.Commands;

namespace SyncTrip.Application.Users.Validators;

/// <summary>
/// Validator pour UpdateUserProfileCommand.
/// Valide les règles métier : username optionnel mais valide si fourni, âge > 14 ans si date modifiée.
/// </summary>
public class UpdateUserProfileValidator : AbstractValidator<UpdateUserProfileCommand>
{
    /// <summary>
    /// Âge minimum requis pour utiliser l'application.
    /// </summary>
    private const int MinimumAge = 14;

    public UpdateUserProfileValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("L'identifiant utilisateur est obligatoire");

        RuleFor(x => x.Username)
            .MaximumLength(50).WithMessage("Le pseudo ne peut pas dépasser 50 caractères")
            .Matches(@"^[a-zA-Z0-9_-]+$")
            .WithMessage("Le pseudo ne peut contenir que des lettres, chiffres, tirets et underscores")
            .When(x => !string.IsNullOrWhiteSpace(x.Username));

        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("Le prénom ne peut pas dépasser 100 caractères")
            .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Le nom ne peut pas dépasser 100 caractères")
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));

        RuleFor(x => x.BirthDate)
            .Must(BeOlderThan14)
            .WithMessage($"Vous devez avoir plus de {MinimumAge} ans pour utiliser cette application")
            .When(x => x.BirthDate.HasValue);

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(500).WithMessage("L'URL de l'avatar ne peut pas dépasser 500 caractères")
            .When(x => !string.IsNullOrWhiteSpace(x.AvatarUrl));

        RuleFor(x => x.LicenseTypes)
            .Must(types => types == null || types.All(t => Enum.IsDefined(typeof(Core.Enums.LicenseType), t)))
            .WithMessage("Un ou plusieurs types de permis sont invalides")
            .When(x => x.LicenseTypes != null);
    }

    /// <summary>
    /// Vérifie que l'utilisateur a plus de 14 ans.
    /// </summary>
    private static bool BeOlderThan14(DateOnly? birthDate)
    {
        if (!birthDate.HasValue)
            return true;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        int age = today.Year - birthDate.Value.Year;

        // Ajuster si l'anniversaire n'est pas encore passé cette année
        if (birthDate.Value > today.AddYears(-age))
            age--;

        return age > MinimumAge;
    }
}
