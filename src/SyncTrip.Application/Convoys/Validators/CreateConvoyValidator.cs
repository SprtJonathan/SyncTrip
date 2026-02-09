using FluentValidation;
using SyncTrip.Application.Convoys.Commands;

namespace SyncTrip.Application.Convoys.Validators;

/// <summary>
/// Validator pour CreateConvoyCommand.
/// </summary>
public class CreateConvoyValidator : AbstractValidator<CreateConvoyCommand>
{
    public CreateConvoyValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("L'identifiant utilisateur est obligatoire.");

        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage("Un véhicule est obligatoire pour créer un convoi.");
    }
}
