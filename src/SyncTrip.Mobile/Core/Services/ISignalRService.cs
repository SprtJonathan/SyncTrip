namespace SyncTrip.Mobile.Core.Services;

/// <summary>
/// Service de connexion temps réel via SignalR (TripHub).
/// Gère la diffusion et la réception des positions GPS.
/// </summary>
public interface ISignalRService
{
    /// <summary>
    /// Indique si la connexion SignalR est active.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Événement déclenché à la réception d'une position GPS d'un membre.
    /// Paramètres : userId, latitude, longitude, timestamp.
    /// </summary>
    event Action<string, double, double, DateTime>? LocationReceived;

    /// <summary>
    /// Établit la connexion au TripHub et rejoint le groupe du voyage.
    /// </summary>
    Task ConnectAsync(Guid tripId);

    /// <summary>
    /// Envoie la position GPS de l'utilisateur aux autres membres.
    /// </summary>
    Task SendLocationAsync(Guid tripId, double latitude, double longitude);

    /// <summary>
    /// Quitte le groupe et ferme la connexion SignalR.
    /// </summary>
    Task DisconnectAsync();
}
