using FluentValidation;
using SyncTrip.Application.Navigation.Queries;

namespace SyncTrip.Application.Navigation.Validators;

public class SearchAddressValidator : AbstractValidator<SearchAddressQuery>
{
    public SearchAddressValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty().WithMessage("La requete de recherche est obligatoire.")
            .MinimumLength(2).WithMessage("La requete doit contenir au moins 2 caracteres.")
            .MaximumLength(200).WithMessage("La requete ne doit pas depasser 200 caracteres.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 10).WithMessage("Le nombre de resultats doit etre entre 1 et 10.");
    }
}
