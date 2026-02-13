using FluentValidation;
using SyncTrip.Application.Navigation.Commands;

namespace SyncTrip.Application.Navigation.Validators;

public class CalculateTripRouteValidator : AbstractValidator<CalculateTripRouteCommand>
{
    public CalculateTripRouteValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty().WithMessage("L'identifiant du voyage est obligatoire.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("L'identifiant utilisateur est obligatoire.");
    }
}
