using FluentValidation;
using SyncTrip.Application.Navigation.Queries;

namespace SyncTrip.Application.Navigation.Validators;

public class CalculateRouteValidator : AbstractValidator<CalculateRouteQuery>
{
    public CalculateRouteValidator()
    {
        RuleFor(x => x.RouteProfile)
            .IsInEnum().WithMessage("Le profil de route est invalide.");

        RuleFor(x => x.Waypoints)
            .NotEmpty().WithMessage("Au moins 2 waypoints sont necessaires.")
            .Must(w => w.Count >= 2).WithMessage("Au moins 2 waypoints sont necessaires pour calculer un itineraire.");
    }
}
