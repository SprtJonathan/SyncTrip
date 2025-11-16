using FluentValidation;
using SyncTrip.Api.Application.DTOs.Convoys;

namespace SyncTrip.Api.Application.Validators;

/// <summary>
/// Validator pour rejoindre un convoi
/// </summary>
public class JoinConvoyRequestValidator : AbstractValidator<JoinConvoyRequest>
{
    public JoinConvoyRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Le code du convoi est obligatoire")
            .Length(6).WithMessage("Le code doit contenir exactement 6 caractères")
            .Matches(@"^[A-Za-z0-9]{6}$").WithMessage("Le code doit contenir uniquement des lettres et chiffres");

        RuleFor(x => x.VehicleName)
            .MaximumLength(100).WithMessage("Le nom du véhicule ne peut pas dépasser 100 caractères")
            .When(x => !string.IsNullOrEmpty(x.VehicleName));
    }
}
