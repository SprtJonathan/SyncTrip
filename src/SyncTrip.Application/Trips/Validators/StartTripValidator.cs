using FluentValidation;
using SyncTrip.Application.Trips.Commands;
using SyncTrip.Core.Enums;

namespace SyncTrip.Application.Trips.Validators;

/// <summary>
/// Validator pour StartTripCommand.
/// </summary>
public class StartTripValidator : AbstractValidator<StartTripCommand>
{
    public StartTripValidator()
    {
        RuleFor(x => x.ConvoyId)
            .NotEmpty().WithMessage("L'identifiant du convoi est obligatoire.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("L'identifiant utilisateur est obligatoire.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Le statut du voyage est invalide.")
            .Must(s => s != TripStatus.Finished).WithMessage("Un voyage ne peut pas être créé avec le statut Terminé.");

        RuleFor(x => x.RouteProfile)
            .IsInEnum().WithMessage("Le profil de route est invalide.");
    }
}
