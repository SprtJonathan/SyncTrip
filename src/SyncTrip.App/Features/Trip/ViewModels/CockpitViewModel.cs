using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.App.Core.Platform;
using SyncTrip.App.Core.Services;
using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.App.Features.Trip.ViewModels;

public partial class CockpitViewModel : ObservableObject
{
    private readonly ITripService _tripService;
    private readonly ISignalRService _signalRService;
    private readonly ILocationService _locationService;
    private readonly INavigationService _navigationService;
    private DispatcherTimer? _locationTimer;
    private DispatcherTimer? _durationTimer;

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

    public event Action? PositionsUpdated;

    public CockpitViewModel(ITripService tripService, ISignalRService signalRService,
        ILocationService locationService, INavigationService navigationService)
    {
        _tripService = tripService;
        _signalRService = signalRService;
        _locationService = locationService;
        _navigationService = navigationService;
    }

    public void Initialize(string convoyId, string tripId)
    {
        ConvoyId = convoyId;
        TripId = tripId;
    }

    [RelayCommand]
    public async Task LoadTrip()
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

        _signalRService.LocationReceived += OnLocationReceived;
        await _signalRService.ConnectAsync(tripGuid);

        _locationTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        _locationTimer.Tick += async (s, e) => await UpdateLocation();
        _locationTimer.Start();

        if (Trip is not null)
        {
            _durationTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _durationTimer.Tick += (s, e) =>
            {
                var elapsed = DateTime.UtcNow - Trip.StartTime;
                TripDuration = elapsed.ToString(@"hh\:mm\:ss");
            };
            _durationTimer.Start();
        }

        IsTracking = true;

        await UpdateLocation();
    }

    private async Task UpdateLocation()
    {
        try
        {
            var location = await _locationService.GetCurrentLocationAsync();
            if (location is null) return;

            UserLatitude = location.Latitude;
            UserLongitude = location.Longitude;

            if (Guid.TryParse(TripId, out var tripGuid))
                await _signalRService.SendLocationAsync(tripGuid, location.Latitude, location.Longitude);

            PositionsUpdated?.Invoke();
        }
        catch
        {
            // Geolocalisation peut echouer silencieusement
        }
    }

    private void OnLocationReceived(string userId, double lat, double lon, DateTime timestamp)
    {
        Dispatcher.UIThread.Post(() =>
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
        await _navigationService.GoBackAsync();
    }

    public async Task StopTracking()
    {
        _locationTimer?.Stop();
        _durationTimer?.Stop();
        _signalRService.LocationReceived -= OnLocationReceived;
        await _signalRService.DisconnectAsync();
        IsTracking = false;
    }

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
