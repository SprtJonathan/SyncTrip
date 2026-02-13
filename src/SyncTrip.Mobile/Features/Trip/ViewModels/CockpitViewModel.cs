using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.Mobile.Core.Services;
using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.Mobile.Features.Trip.ViewModels;

/// <summary>
/// ViewModel pour la page cockpit (carte GPS temps réel).
/// Gère la géolocalisation, SignalR et l'affichage des positions.
/// </summary>
[QueryProperty(nameof(ConvoyId), "convoyId")]
[QueryProperty(nameof(TripId), "tripId")]
public partial class CockpitViewModel : ObservableObject
{
    private readonly ITripService _tripService;
    private readonly ISignalRService _signalRService;
    private IDispatcherTimer? _locationTimer;

    [ObservableProperty]
    private string convoyId = string.Empty;

    [ObservableProperty]
    private string tripId = string.Empty;

    [ObservableProperty]
    private TripDetailsDto? trip;

    [ObservableProperty]
    private double userLatitude;

    [ObservableProperty]
    private double userLongitude;

    [ObservableProperty]
    private bool isTracking;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string tripDuration = "00:00:00";

    [ObservableProperty]
    private ObservableCollection<MemberPosition> memberPositions = new();

    /// <summary>
    /// Événement déclenché quand les positions changent (pour rafraîchir la carte).
    /// </summary>
    public event Action? PositionsUpdated;

    public CockpitViewModel(ITripService tripService, ISignalRService signalRService)
    {
        _tripService = tripService;
        _signalRService = signalRService;
    }

    [RelayCommand]
    private async Task LoadTrip()
    {
        try
        {
            ErrorMessage = null;

            if (!Guid.TryParse(ConvoyId, out var convoyGuid) || !Guid.TryParse(TripId, out var tripGuid))
            {
                ErrorMessage = "Parametres de navigation invalides.";
                return;
            }

            Trip = await _tripService.GetTripByIdAsync(convoyGuid, tripGuid);
            if (Trip is null)
            {
                ErrorMessage = "Impossible de charger le voyage.";
                return;
            }

            await StartTracking();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
    }

    private async Task StartTracking()
    {
        if (!Guid.TryParse(TripId, out var tripGuid))
            return;

        // Connexion SignalR
        _signalRService.LocationReceived += OnLocationReceived;
        await _signalRService.ConnectAsync(tripGuid);

        // Timer géolocalisation (5 secondes)
        _locationTimer = Application.Current?.Dispatcher.CreateTimer();
        if (_locationTimer is not null)
        {
            _locationTimer.Interval = TimeSpan.FromSeconds(5);
            _locationTimer.Tick += async (s, e) => await UpdateLocation();
            _locationTimer.Start();
        }

        // Timer durée voyage
        var durationTimer = Application.Current?.Dispatcher.CreateTimer();
        if (durationTimer is not null && Trip is not null)
        {
            durationTimer.Interval = TimeSpan.FromSeconds(1);
            durationTimer.Tick += (s, e) =>
            {
                var elapsed = DateTime.UtcNow - Trip.StartTime;
                TripDuration = elapsed.ToString(@"hh\:mm\:ss");
            };
            durationTimer.Start();
        }

        IsTracking = true;

        // Première localisation immédiate
        await UpdateLocation();
    }

    private async Task UpdateLocation()
    {
        try
        {
            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Best,
                Timeout = TimeSpan.FromSeconds(10)
            });

            if (location is null) return;

            UserLatitude = location.Latitude;
            UserLongitude = location.Longitude;

            if (Guid.TryParse(TripId, out var tripGuid))
                await _signalRService.SendLocationAsync(tripGuid, location.Latitude, location.Longitude);

            PositionsUpdated?.Invoke();
        }
        catch
        {
            // Géolocalisation peut échouer silencieusement
        }
    }

    private void OnLocationReceived(string userId, double lat, double lon, DateTime timestamp)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var existing = MemberPositions.FirstOrDefault(m => m.UserId == userId);
            if (existing is not null)
            {
                existing.Latitude = lat;
                existing.Longitude = lon;
                existing.LastUpdate = timestamp;
            }
            else
            {
                MemberPositions.Add(new MemberPosition
                {
                    UserId = userId,
                    Latitude = lat,
                    Longitude = lon,
                    LastUpdate = timestamp
                });
            }

            PositionsUpdated?.Invoke();
        });
    }

    [RelayCommand]
    private async Task LeaveCockpit()
    {
        await StopTracking();
        await Shell.Current.GoToAsync("..");
    }

    public async Task StopTracking()
    {
        _locationTimer?.Stop();
        _signalRService.LocationReceived -= OnLocationReceived;
        await _signalRService.DisconnectAsync();
        IsTracking = false;
    }

    /// <summary>
    /// Position d'un membre du convoi.
    /// </summary>
    public class MemberPosition : ObservableObject
    {
        public string UserId { get; set; } = string.Empty;

        private double _latitude;
        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        private double _longitude;
        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        private DateTime _lastUpdate;
        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set => SetProperty(ref _lastUpdate, value);
        }
    }
}
