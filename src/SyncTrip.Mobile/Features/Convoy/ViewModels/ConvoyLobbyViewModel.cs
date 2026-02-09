using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.Mobile.Core.Services;
using SyncTrip.Shared.DTOs.Convoys;

namespace SyncTrip.Mobile.Features.Convoy.ViewModels;

/// <summary>
/// ViewModel pour le lobby d'un convoi (liste des convois de l'utilisateur).
/// </summary>
public partial class ConvoyLobbyViewModel : ObservableObject
{
    private readonly IConvoyService _convoyService;

    [ObservableProperty]
    private ObservableCollection<ConvoyDto> convoys = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isEmpty;

    public ConvoyLobbyViewModel(IConvoyService convoyService)
    {
        _convoyService = convoyService;
    }

    [RelayCommand]
    private async Task LoadConvoys()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var convoyList = await _convoyService.GetMyConvoysAsync();

            Convoys.Clear();
            foreach (var convoy in convoyList)
            {
                Convoys.Add(convoy);
            }

            IsEmpty = Convoys.Count == 0;
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
    private async Task CreateConvoy()
    {
        try
        {
            await Shell.Current.GoToAsync("createconvoy");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur de navigation: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task JoinConvoy()
    {
        try
        {
            await Shell.Current.GoToAsync("joinconvoy");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur de navigation: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task LeaveConvoy(string joinCode)
    {
        try
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Confirmation",
                "Voulez-vous vraiment quitter ce convoi ?",
                "Oui",
                "Non"
            );

            if (!confirm)
                return;

            var success = await _convoyService.LeaveConvoyAsync(joinCode);

            if (success)
            {
                var convoy = Convoys.FirstOrDefault(c => c.JoinCode == joinCode);
                if (convoy != null)
                    Convoys.Remove(convoy);

                IsEmpty = Convoys.Count == 0;
            }
            else
            {
                ErrorMessage = "Impossible de quitter le convoi.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadConvoys();
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
