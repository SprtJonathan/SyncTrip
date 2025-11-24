using SyncTrip.Core.Entities;

namespace SyncTrip.Core.Interfaces;

/// <summary>
/// Interface du repository pour les véhicules.
/// </summary>
public interface IVehicleRepository
{
    /// <summary>
    /// Récupère un véhicule par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant du véhicule.</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>Le véhicule ou null si non trouvé.</returns>
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Récupère tous les véhicules d'un utilisateur.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>Liste des véhicules de l'utilisateur.</returns>
    Task<IList<Vehicle>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Ajoute un nouveau véhicule.
    /// </summary>
    /// <param name="vehicle">Véhicule à ajouter.</param>
    /// <param name="ct">Token d'annulation.</param>
    Task AddAsync(Vehicle vehicle, CancellationToken ct = default);

    /// <summary>
    /// Met à jour un véhicule existant.
    /// </summary>
    /// <param name="vehicle">Véhicule à mettre à jour.</param>
    /// <param name="ct">Token d'annulation.</param>
    Task UpdateAsync(Vehicle vehicle, CancellationToken ct = default);

    /// <summary>
    /// Supprime un véhicule.
    /// </summary>
    /// <param name="vehicle">Véhicule à supprimer.</param>
    /// <param name="ct">Token d'annulation.</param>
    Task DeleteAsync(Vehicle vehicle, CancellationToken ct = default);
}
