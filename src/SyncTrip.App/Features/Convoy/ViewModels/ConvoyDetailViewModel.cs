using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.App.Core.Platform;
using SyncTrip.App.Core.Services;
using SyncTrip.Shared.DTOs.Convoys;
using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.App.Features.Convoy.ViewModels;

public partial class ConvoyDetailViewModel : ObservableObject
{
    private readonly IConvoyService _convoyService;
    private readonly ITripService _tripService;
    private readonly IUserService _userService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string convoyId = string.Empty;

    [ObservableProperty]
    private string joinCode = string.Empty;

    [ObservableProperty]
    private ConvoyDetailsDto? convoy;

    [ObservableProperty]
    private TripDetailsDto? activeTrip;

    [ObservableProperty]
    private bool isLeader;

    [ObservableProperty]
    private bool hasActiveTrip;

    [ObservableProperty]
    private bool canStartTrip;

    [ObservableProperty]
    private bool canShowStartTripButton;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool showStartTripForm;

    [ObservableProperty]
    private string destinationName = string.Empty;

    [ObservableProperty]
    private string destinationLatitude = string.Empty;

    [ObservableProperty]
    private string destinationLongitude = string.Empty;

    public ConvoyDetailViewModel(IConvoyService convoyService, ITripService tripService, IUserService userService,
        INavigationService navigationService, IDialogService dialogService)
    {
        _convoyService = convoyService;
        _tripService = tripService;
        _userService = userService;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    partial void OnCanStartTripChanged(bool value) => CanShowStartTripButton = value && !ShowStartTripForm;
    partial void OnShowStartTripFormChanged(bool value) => CanShowStartTripButton = CanStartTrip && !value;

    public void Initialize(string convoyId, string joinCode)
    {
        ConvoyId = convoyId;
        JoinCode = joinCode;
    }

    [RelayCommand]
    public async Task LoadDetails()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var convoyDetails = await _convoyService.GetConvoyByCodeAsync(JoinCode);
            if (convoyDetails is null)
            {
                ErrorMessage = "Impossible de charger les details du convoi.";
                return;
            }

            Convoy = convoyDetails;

            var profile = await _userService.GetProfileAsync();
            IsLeader = profile is not null && convoyDetails.LeaderUserId == profile.Id;

            var trip = await _tripService.GetActiveTripAsync(convoyDetails.Id);
            ActiveTrip = trip;
            HasActiveTrip = trip is not null;
            CanStartTrip = IsLeader && !HasActiveTrip;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ToggleStartTripForm()
    {
        ShowStartTripForm = !ShowStartTripForm;
        ErrorMessage = null;
    }

    [RelayCommand]
    private async Task StartTrip()
    {
        try
        {
            if (Convoy is null) return;

            if (string.IsNullOrWhiteSpace(DestinationName))
            {
                ErrorMessage = "Veuillez entrer un nom de destination.";
                return;
            }

            if (!double.TryParse(DestinationLatitude, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out var lat) ||
                lat < -90 || lat > 90)
            {
                ErrorMessage = "Latitude invalide (entre -90 et 90).";
                return;
            }

            if (!double.TryParse(DestinationLongitude, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out var lon) ||
                lon < -180 || lon > 180)
            {
                ErrorMessage = "Longitude invalide (entre -180 et 180).";
                return;
            }

            IsLoading = true;
            ErrorMessage = null;

            var request = new StartTripRequest
            {
                Status = 1,
                RouteProfile = 1,
                Waypoints = new List<CreateWaypointRequest>
                {
                    new()
                    {
                        OrderIndex = 0,
                        Latitude = lat,
                        Longitude = lon,
                        Name = DestinationName.Trim(),
                        Type = 3 // Destination
                    }
                }
            };

            var tripId = await _tripService.StartTripAsync(Convoy.Id, request);
            if (tripId is null)
            {
                ErrorMessage = "Impossible de demarrer le voyage.";
                return;
            }

            ShowStartTripForm = false;

            await _navigationService.NavigateToAsync("cockpit", new Dictionary<string, string>
            {
                ["convoyId"] = Convoy.Id.ToString(),
                ["tripId"] = tripId.ToString()!
            });
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task OpenCockpit()
    {
        if (Convoy is null || ActiveTrip is null) return;

        await _navigationService.NavigateToAsync("cockpit", new Dictionary<string, string>
        {
            ["convoyId"] = Convoy.Id.ToString(),
            ["tripId"] = ActiveTrip.Id.ToString()
        });
    }

    [RelayCommand]
    private async Task OpenChat()
    {
        if (Convoy is null) return;

        await _navigationService.NavigateToAsync("chat", new Dictionary<string, string>
        {
            ["convoyId"] = Convoy.Id.ToString()
        });
    }

    [RelayCommand]
    private async Task EndTrip()
    {
        if (Convoy is null || ActiveTrip is null) return;

        try
        {
            var confirm = await _dialogService.ConfirmAsync(
                "Confirmation",
                "Voulez-vous vraiment terminer ce voyage ?");

            if (!confirm) return;

            IsLoading = true;
            ErrorMessage = null;

            var success = await _tripService.EndTripAsync(Convoy.Id, ActiveTrip.Id);
            if (success)
            {
                ActiveTrip = null;
                HasActiveTrip = false;
                CanStartTrip = IsLeader;
            }
            else
            {
                ErrorMessage = "Impossible de terminer le voyage.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public static string GetRoleName(int role)
    {
        return role switch
        {
            2 => "Leader",
            _ => "Membre"
        };
    }
}
