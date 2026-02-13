using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.App.Core.Platform;
using SyncTrip.App.Core.Services;
using SyncTrip.Shared.DTOs.Convoys;
using SyncTrip.Shared.DTOs.Navigation;
using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.App.Features.Convoy.ViewModels;

public partial class ConvoyDetailViewModel : ObservableObject
{
    private readonly IConvoyService _convoyService;
    private readonly ITripService _tripService;
    private readonly IUserService _userService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly INavigationApiService _navigationApiService;

    private CancellationTokenSource? _searchCts;

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
    private string? activeTripDestination;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool showStartTripForm;

    [ObservableProperty]
    private int selectedRouteProfile = 1;

    [ObservableProperty]
    private string destinationName = string.Empty;

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private AddressResultDto? selectedAddress;

    [ObservableProperty]
    private bool isSearching;

    [ObservableProperty]
    private bool showSearchResults;

    public ObservableCollection<AddressResultDto> SearchResults { get; } = [];

    public ConvoyDetailViewModel(IConvoyService convoyService, ITripService tripService, IUserService userService,
        INavigationService navigationService, IDialogService dialogService, INavigationApiService navigationApiService)
    {
        _convoyService = convoyService;
        _tripService = tripService;
        _userService = userService;
        _navigationService = navigationService;
        _dialogService = dialogService;
        _navigationApiService = navigationApiService;
    }

    partial void OnCanStartTripChanged(bool value) => CanShowStartTripButton = value && !ShowStartTripForm;
    partial void OnShowStartTripFormChanged(bool value) => CanShowStartTripButton = CanStartTrip && !value;

    partial void OnSearchQueryChanged(string value)
    {
        _searchCts?.Cancel();

        if (SelectedAddress is not null && value == SelectedAddress.DisplayName)
            return;

        SelectedAddress = null;

        if (string.IsNullOrWhiteSpace(value) || value.Length < 2)
        {
            SearchResults.Clear();
            ShowSearchResults = false;
            return;
        }

        _searchCts = new CancellationTokenSource();
        _ = SearchAddressWithDebounceAsync(value, _searchCts.Token);
    }

    private async Task SearchAddressWithDebounceAsync(string query, CancellationToken ct)
    {
        try
        {
            IsSearching = true;
            await Task.Delay(400, ct);

            var results = await _navigationApiService.SearchAddressAsync(query, 5, ct);

            ct.ThrowIfCancellationRequested();

            SearchResults.Clear();
            foreach (var result in results)
                SearchResults.Add(result);

            ShowSearchResults = SearchResults.Count > 0;
        }
        catch (OperationCanceledException)
        {
            // Debounce cancelled — ignored
        }
        finally
        {
            IsSearching = false;
        }
    }

    [RelayCommand]
    private void SelectAddress(AddressResultDto address)
    {
        SelectedAddress = address;
        SearchQuery = address.DisplayName;
        SearchResults.Clear();
        ShowSearchResults = false;

        if (string.IsNullOrWhiteSpace(DestinationName))
            DestinationName = address.DisplayName;
    }

    [RelayCommand]
    private void ClearAddress()
    {
        SelectedAddress = null;
        SearchQuery = string.Empty;
        SearchResults.Clear();
        ShowSearchResults = false;
    }

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
            ActiveTripDestination = trip?.Waypoints.FirstOrDefault(w => w.Type == 3)?.Name;
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

            if (SelectedAddress is null)
            {
                ErrorMessage = "Veuillez rechercher et selectionner une adresse.";
                return;
            }

            IsLoading = true;
            ErrorMessage = null;

            var name = string.IsNullOrWhiteSpace(DestinationName)
                ? SelectedAddress.DisplayName
                : DestinationName.Trim();

            var request = new StartTripRequest
            {
                Status = 1,
                RouteProfile = SelectedRouteProfile,
                Waypoints = new List<CreateWaypointRequest>
                {
                    new()
                    {
                        OrderIndex = 0,
                        Latitude = SelectedAddress.Latitude,
                        Longitude = SelectedAddress.Longitude,
                        Name = name,
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
