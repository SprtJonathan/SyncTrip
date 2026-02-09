using FluentValidation;
using SyncTrip.Application.Trips.Commands;

namespace SyncTrip.Application.Trips.Validators;

/// <summary>
/// Validator pour AddWaypointCommand.
/// </summary>
public class AddWaypointValidator : AbstractValidator<AddWaypointCommand>
{
    public AddWaypointValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty().WithMessage("L'identifiant du voyage est obligatoire.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("L'identifiant utilisateur est obligatoire.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("La latitude doit être comprise entre -90 et 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("La longitude doit être comprise entre -180 et 180.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Le nom du point de passage est obligatoire.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Le type de waypoint est invalide.");
    }
}
