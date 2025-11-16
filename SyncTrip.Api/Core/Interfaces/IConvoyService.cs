using SyncTrip.Api.Application.DTOs.Convoys;

namespace SyncTrip.Api.Core.Interfaces;

/// <summary>
/// Service de gestion des convois
/// </summary>
public interface IConvoyService
{
    /// <summary>
    /// Crée un nouveau convoi
    /// </summary>
    Task<ConvoyDto> CreateConvoyAsync(Guid userId, CreateConvoyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rejoint un convoi existant
    /// </summary>
    Task<ConvoyDto> JoinConvoyAsync(Guid userId, JoinConvoyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Quitte un convoi
    /// </summary>
    Task LeaveConvoyAsync(Guid userId, Guid convoyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un convoi par son ID
    /// </summary>
    Task<ConvoyDto?> GetConvoyByIdAsync(Guid convoyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un convoi par son code
    /// </summary>
    Task<ConvoyDto?> GetConvoyByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère tous les convois d'un utilisateur
    /// </summary>
    Task<IEnumerable<ConvoyDto>> GetUserConvoysAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un convoi
    /// </summary>
    Task<ConvoyDto> UpdateConvoyAsync(Guid convoyId, UpdateConvoyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un convoi (soft delete)
    /// </summary>
    Task DeleteConvoyAsync(Guid convoyId, CancellationToken cancellationToken = default);
}
