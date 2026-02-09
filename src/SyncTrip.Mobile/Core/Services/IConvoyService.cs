using SyncTrip.Shared.DTOs.Convoys;

namespace SyncTrip.Mobile.Core.Services;

/// <summary>
/// Service de gestion des convois.
/// </summary>
public interface IConvoyService
{
    /// <summary>
    /// Crée un nouveau convoi.
    /// </summary>
    Task<Guid?> CreateConvoyAsync(CreateConvoyRequest request, CancellationToken ct = default);

    /// <summary>
    /// Récupère les détails d'un convoi par code.
    /// </summary>
    Task<ConvoyDetailsDto?> GetConvoyByCodeAsync(string joinCode, CancellationToken ct = default);

    /// <summary>
    /// Liste les convois de l'utilisateur connecté.
    /// </summary>
    Task<List<ConvoyDto>> GetMyConvoysAsync(CancellationToken ct = default);

    /// <summary>
    /// Rejoint un convoi existant.
    /// </summary>
    Task<bool> JoinConvoyAsync(string joinCode, JoinConvoyRequest request, CancellationToken ct = default);

    /// <summary>
    /// Quitte un convoi.
    /// </summary>
    Task<bool> LeaveConvoyAsync(string joinCode, CancellationToken ct = default);

    /// <summary>
    /// Dissout un convoi (leader uniquement).
    /// </summary>
    Task<bool> DissolveConvoyAsync(string joinCode, CancellationToken ct = default);
}
