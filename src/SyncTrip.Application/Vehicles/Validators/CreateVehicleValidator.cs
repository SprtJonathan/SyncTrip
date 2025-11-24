using FluentValidation;
using SyncTrip.Application.Vehicles.Commands;
using SyncTrip.Core.Enums;

namespace SyncTrip.Application.Vehicles.Validators;

/// <summary>
/// Validator pour CreateVehicleCommand.
/// Valide les règles métier : marque valide, modèle obligatoire, type valide, année cohérente.
/// </summary>
public class CreateVehicleValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("L'identifiant utilisateur est obligatoire");

        RuleFor(x => x.BrandId)
            .GreaterThan(0).WithMessage("L'identifiant de la marque est invalide");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Le modèle du véhicule est obligatoire")
            .MaximumLength(100).WithMessage("Le modèle ne peut pas dépasser 100 caractères");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Le type de véhicule est invalide");

        RuleFor(x => x.Color)
            .MaximumLength(50).WithMessage("La couleur ne peut pas dépasser 50 caractères")
            .When(x => !string.IsNullOrWhiteSpace(x.Color));

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 1)
            .WithMessage($"L'année doit être comprise entre 1900 et {DateTime.UtcNow.Year + 1}")
            .When(x => x.Year.HasValue);
    }
}
