using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.Mobile.Core.Services;
using SyncTrip.Shared.DTOs.Convoys;
using SyncTrip.Shared.DTOs.Trips;

namespace SyncTrip.Mobile.Features.Convoy.ViewModels;

/// <summary>
/// ViewModel pour la page de détail d'un convoi.
/// Affiche les membres et gère le cycle de vie des voyages.
/// </summary>
[QueryProperty(nameof(ConvoyId), "convoyId")]
[QueryProperty(nameof(JoinCode), "joinCode")]
public partial class ConvoyDetailViewModel : ObservableObject
{
    private readonly IConvoyService _convoyService;
    private readonly ITripService _tripService;
    private readonly IUserService _userService;

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
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    public ConvoyDetailViewModel(IConvoyService convoyService, ITripService tripService, IUserService userService)
    {
        _convoyService = convoyService;
        _tripService = tripService;
        _userService = userService;
    }

    [RelayCommand]
    private async Task LoadDetails()
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
    private async Task StartTrip()
    {
        try
        {
            if (Convoy is null) return;

            IsLoading = true;
            ErrorMessage = null;

            var request = new StartTripRequest
            {
                Status = 1, // Recording
                RouteProfile = 1 // Fast
            };

            var tripId = await _tripService.StartTripAsync(Convoy.Id, request);
            if (tripId is null)
            {
                ErrorMessage = "Impossible de demarrer le voyage.";
                return;
            }

            await Shell.Current.GoToAsync($"cockpit?convoyId={Convoy.Id}&tripId={tripId}");
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

        await Shell.Current.GoToAsync($"cockpit?convoyId={Convoy.Id}&tripId={ActiveTrip.Id}");
    }

    [RelayCommand]
    private async Task EndTrip()
    {
        if (Convoy is null || ActiveTrip is null) return;

        try
        {
            var confirm = await Shell.Current.DisplayAlertAsync(
                "Confirmation",
                "Voulez-vous vraiment terminer ce voyage ?",
                "Oui",
                "Non"
            );

            if (!confirm) return;

            IsLoading = true;
            ErrorMessage = null;

            var success = await _tripService.EndTripAsync(Convoy.Id, ActiveTrip.Id);
            if (success)
            {
                ActiveTrip = null;
                HasActiveTrip = false;
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

    /// <summary>
    /// Retourne le nom du rôle en français.
    /// </summary>
    public static string GetRoleName(int role)
    {
        return role switch
        {
            2 => "Leader",
            _ => "Membre"
        };
    }
}
