using FluentValidation;
using SyncTrip.Api.Application.DTOs.Convoys;

namespace SyncTrip.Api.Application.Validators;

/// <summary>
/// Validator pour la création de convoi
/// </summary>
public class CreateConvoyRequestValidator : AbstractValidator<CreateConvoyRequest>
{
    public CreateConvoyRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Le nom ne peut pas dépasser 100 caractères")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.VehicleName)
            .MaximumLength(100).WithMessage("Le nom du véhicule ne peut pas dépasser 100 caractères")
            .When(x => !string.IsNullOrEmpty(x.VehicleName));
    }
}
