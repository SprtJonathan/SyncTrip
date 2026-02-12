using SyncTrip.Shared.DTOs.Chat;

namespace SyncTrip.Application.Chat.Services;

/// <summary>
/// Interface pour les notifications temps réel liées aux convois (chat).
/// </summary>
public interface IConvoyNotificationService
{
    /// <summary>
    /// Notifie les membres du convoi qu'un nouveau message a été envoyé.
    /// </summary>
    Task NotifyNewMessageAsync(Guid convoyId, MessageDto message, CancellationToken ct = default);
}
