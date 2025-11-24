using FluentValidation;
using SyncTrip.Application.Vehicles.Commands;

namespace SyncTrip.Application.Vehicles.Validators;

/// <summary>
/// Validator pour UpdateVehicleCommand.
/// Valide les règles métier : modèle valide si fourni, année cohérente.
/// </summary>
public class UpdateVehicleValidator : AbstractValidator<UpdateVehicleCommand>
{
    public UpdateVehicleValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage("L'identifiant du véhicule est obligatoire");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("L'identifiant utilisateur est obligatoire");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Le modèle ne peut pas être vide")
            .MaximumLength(100).WithMessage("Le modèle ne peut pas dépasser 100 caractères")
            .When(x => x.Model != null);

        RuleFor(x => x.Color)
            .MaximumLength(50).WithMessage("La couleur ne peut pas dépasser 50 caractères")
            .When(x => !string.IsNullOrWhiteSpace(x.Color));

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 1)
            .WithMessage($"L'année doit être comprise entre 1900 et {DateTime.UtcNow.Year + 1}")
            .When(x => x.Year.HasValue);
    }
}
