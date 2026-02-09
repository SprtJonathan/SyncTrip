using FluentValidation;
using SyncTrip.Application.Convoys.Commands;

namespace SyncTrip.Application.Convoys.Validators;

/// <summary>
/// Validator pour JoinConvoyCommand.
/// </summary>
public class JoinConvoyValidator : AbstractValidator<JoinConvoyCommand>
{
    public JoinConvoyValidator()
    {
        RuleFor(x => x.JoinCode)
            .NotEmpty().WithMessage("Le code du convoi est obligatoire.")
            .Length(6).WithMessage("Le code du convoi doit contenir 6 caractères.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("L'identifiant utilisateur est obligatoire.");

        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage("Un véhicule est obligatoire pour rejoindre un convoi.");
    }
}
