using FluentValidation;
using SyncTrip.Api.Application.DTOs.Locations;

namespace SyncTrip.Api.Application.Validators;

/// <summary>
/// Validator pour la mise à jour de position GPS
/// </summary>
public class UpdateLocationRequestValidator : AbstractValidator<UpdateLocationRequest>
{
    public UpdateLocationRequestValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("La latitude doit être entre -90 et 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("La longitude doit être entre -180 et 180");

        RuleFor(x => x.Altitude)
            .InclusiveBetween(0, 100000).WithMessage("L'altitude doit être entre 0 et 100000 mètres")
            .When(x => x.Altitude.HasValue);

        RuleFor(x => x.Speed)
            .InclusiveBetween(0, 500).WithMessage("La vitesse doit être entre 0 et 500 km/h")
            .When(x => x.Speed.HasValue);

        RuleFor(x => x.Heading)
            .InclusiveBetween(0, 360).WithMessage("Le cap doit être entre 0 et 360 degrés")
            .When(x => x.Heading.HasValue);

        RuleFor(x => x.Accuracy)
            .InclusiveBetween(0, 1000).WithMessage("La précision doit être entre 0 et 1000 mètres")
            .When(x => x.Accuracy.HasValue);
    }
}
