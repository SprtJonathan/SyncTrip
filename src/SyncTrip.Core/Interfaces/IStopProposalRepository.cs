using SyncTrip.Core.Entities;

namespace SyncTrip.Core.Interfaces;

/// <summary>
/// Interface du repository pour les propositions d'arrêt.
/// </summary>
public interface IStopProposalRepository
{
    /// <summary>
    /// Récupère une proposition par son identifiant, avec ses votes, le voyage et les membres du convoi.
    /// </summary>
    Task<StopProposal?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Récupère la proposition en attente d'un voyage (une seule active à la fois).
    /// </summary>
    Task<StopProposal?> GetPendingByTripIdAsync(Guid tripId, CancellationToken ct = default);

    /// <summary>
    /// Récupère toutes les propositions en attente dont le délai est expiré.
    /// </summary>
    Task<IList<StopProposal>> GetExpiredPendingAsync(CancellationToken ct = default);

    /// <summary>
    /// Récupère toutes les propositions d'un voyage (historique).
    /// </summary>
    Task<IList<StopProposal>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default);

    /// <summary>
    /// Ajoute une nouvelle proposition.
    /// </summary>
    Task AddAsync(StopProposal proposal, CancellationToken ct = default);

    /// <summary>
    /// Met à jour une proposition existante.
    /// </summary>
    Task UpdateAsync(StopProposal proposal, CancellationToken ct = default);
}
