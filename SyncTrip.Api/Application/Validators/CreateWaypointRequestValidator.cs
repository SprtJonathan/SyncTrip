using FluentValidation;
using SyncTrip.Api.Application.DTOs.Waypoints;

namespace SyncTrip.Api.Application.Validators;

/// <summary>
/// Validator pour la création de waypoint
/// </summary>
public class CreateWaypointRequestValidator : AbstractValidator<CreateWaypointRequest>
{
    public CreateWaypointRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Le nom est obligatoire")
            .MaximumLength(200).WithMessage("Le nom ne peut pas dépasser 200 caractères");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("La description ne peut pas dépasser 1000 caractères")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("La latitude doit être entre -90 et 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("La longitude doit être entre -180 et 180");
    }
}
