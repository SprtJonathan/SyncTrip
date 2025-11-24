using SyncTrip.Shared.DTOs.Vehicles;

namespace SyncTrip.Mobile.Core.Services;

/// <summary>
/// Service de gestion des véhicules.
/// Permet de créer, lire, modifier et supprimer les véhicules de l'utilisateur.
/// </summary>
public interface IVehicleService
{
    /// <summary>
    /// Récupère la liste des véhicules de l'utilisateur connecté.
    /// </summary>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>Liste des véhicules ou liste vide en cas d'échec.</returns>
    Task<List<VehicleDto>> GetVehiclesAsync(CancellationToken ct = default);

    /// <summary>
    /// Récupère un véhicule spécifique par son identifiant.
    /// </summary>
    /// <param name="vehicleId">Identifiant du véhicule.</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>Véhicule ou null si introuvable.</returns>
    Task<VehicleDto?> GetVehicleByIdAsync(Guid vehicleId, CancellationToken ct = default);

    /// <summary>
    /// Crée un nouveau véhicule pour l'utilisateur connecté.
    /// </summary>
    /// <param name="request">Données de création du véhicule.</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>Identifiant du véhicule créé ou null en cas d'échec.</returns>
    Task<Guid?> CreateVehicleAsync(CreateVehicleRequest request, CancellationToken ct = default);

    /// <summary>
    /// Met à jour un véhicule existant.
    /// </summary>
    /// <param name="vehicleId">Identifiant du véhicule à mettre à jour.</param>
    /// <param name="request">Données de mise à jour.</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>True si la mise à jour a réussi, False sinon.</returns>
    Task<bool> UpdateVehicleAsync(Guid vehicleId, UpdateVehicleRequest request, CancellationToken ct = default);

    /// <summary>
    /// Supprime un véhicule.
    /// </summary>
    /// <param name="vehicleId">Identifiant du véhicule à supprimer.</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>True si la suppression a réussi, False sinon.</returns>
    Task<bool> DeleteVehicleAsync(Guid vehicleId, CancellationToken ct = default);
}
