namespace SyncTrip.Core.Entities;

/// <summary>
/// Représente un vote sur une proposition d'arrêt.
/// </summary>
public class Vote
{
    /// <summary>
    /// Identifiant unique du vote.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identifiant de la proposition d'arrêt.
    /// </summary>
    public Guid StopProposalId { get; private set; }

    /// <summary>
    /// Proposition d'arrêt associée.
    /// </summary>
    public StopProposal StopProposal { get; private set; } = null!;

    /// <summary>
    /// Identifiant de l'utilisateur ayant voté.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Utilisateur ayant voté.
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// True = OUI, False = NON.
    /// </summary>
    public bool IsYes { get; private set; }

    /// <summary>
    /// Date/heure du vote.
    /// </summary>
    public DateTime VotedAt { get; private set; }

    // Constructeur privé pour EF Core
    private Vote()
    {
    }

    /// <summary>
    /// Factory method pour créer un nouveau vote.
    /// </summary>
    /// <param name="stopProposalId">Identifiant de la proposition.</param>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="isYes">True pour OUI, False pour NON.</param>
    /// <returns>Nouvelle instance de Vote.</returns>
    public static Vote Create(Guid stopProposalId, Guid userId, bool isYes)
    {
        if (stopProposalId == Guid.Empty)
            throw new ArgumentException("L'identifiant de la proposition est invalide.", nameof(stopProposalId));

        if (userId == Guid.Empty)
            throw new ArgumentException("L'identifiant de l'utilisateur est invalide.", nameof(userId));

        return new Vote
        {
            Id = Guid.NewGuid(),
            StopProposalId = stopProposalId,
            UserId = userId,
            IsYes = isYes,
            VotedAt = DateTime.UtcNow
        };
    }
}
