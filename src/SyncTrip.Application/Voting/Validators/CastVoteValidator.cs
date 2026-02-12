using FluentValidation;
using SyncTrip.Application.Voting.Commands;

namespace SyncTrip.Application.Voting.Validators;

/// <summary>
/// Validator pour CastVoteCommand.
/// </summary>
public class CastVoteValidator : AbstractValidator<CastVoteCommand>
{
    public CastVoteValidator()
    {
        RuleFor(x => x.ProposalId)
            .NotEmpty().WithMessage("L'identifiant de la proposition est obligatoire.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("L'identifiant utilisateur est obligatoire.");
    }
}
