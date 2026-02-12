using FluentValidation;
using SyncTrip.Application.Voting.Commands;

namespace SyncTrip.Application.Voting.Validators;

/// <summary>
/// Validator pour ProposeStopCommand.
/// </summary>
public class ProposeStopValidator : AbstractValidator<ProposeStopCommand>
{
    public ProposeStopValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty().WithMessage("L'identifiant du voyage est obligatoire.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("L'identifiant utilisateur est obligatoire.");

        RuleFor(x => x.StopType)
            .IsInEnum().WithMessage("Le type d'arrêt est invalide.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("La latitude doit être comprise entre -90 et 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("La longitude doit être comprise entre -180 et 180.");

        RuleFor(x => x.LocationName)
            .NotEmpty().WithMessage("Le nom du lieu est obligatoire.");
    }
}
