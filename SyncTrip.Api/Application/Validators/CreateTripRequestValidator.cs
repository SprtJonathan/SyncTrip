using FluentValidation;
using SyncTrip.Api.Application.DTOs.Trips;

namespace SyncTrip.Api.Application.Validators;

/// <summary>
/// Validator pour la création de trip
/// </summary>
public class CreateTripRequestValidator : AbstractValidator<CreateTripRequest>
{
    public CreateTripRequestValidator()
    {
        RuleFor(x => x.ConvoyId)
            .NotEmpty().WithMessage("L'ID du convoi est obligatoire");

        RuleFor(x => x.Destination)
            .NotEmpty().WithMessage("La destination est obligatoire")
            .MaximumLength(500).WithMessage("La destination ne peut pas dépasser 500 caractères");

        RuleFor(x => x.DestinationLatitude)
            .InclusiveBetween(-90, 90).WithMessage("La latitude doit être entre -90 et 90");

        RuleFor(x => x.DestinationLongitude)
            .InclusiveBetween(-180, 180).WithMessage("La longitude doit être entre -180 et 180");

        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Le nom ne peut pas dépasser 200 caractères")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.PlannedArrivalTime)
            .GreaterThan(x => x.PlannedDepartureTime ?? DateTime.UtcNow)
            .When(x => x.PlannedDepartureTime.HasValue && x.PlannedArrivalTime.HasValue)
            .WithMessage("L'heure d'arrivée prévue doit être postérieure à l'heure de départ prévue");
    }
}
