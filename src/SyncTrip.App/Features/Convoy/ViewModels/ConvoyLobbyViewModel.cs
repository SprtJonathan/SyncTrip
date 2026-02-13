using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.App.Core.Platform;
using SyncTrip.App.Core.Services;
using SyncTrip.Shared.DTOs.Convoys;

namespace SyncTrip.App.Features.Convoy.ViewModels;

public partial class ConvoyLobbyViewModel : ObservableObject
{
    private readonly IConvoyService _convoyService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<ConvoyDto> convoys = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isEmpty;

    public ConvoyLobbyViewModel(IConvoyService convoyService, INavigationService navigationService, IDialogService dialogService)
    {
        _convoyService = convoyService;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    public async Task LoadConvoys()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var convoyList = await _convoyService.GetMyConvoysAsync();

            Convoys.Clear();
            foreach (var convoy in convoyList)
                Convoys.Add(convoy);

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
        await _navigationService.NavigateToAsync("createconvoy");
    }

    [RelayCommand]
    private async Task JoinConvoy()
    {
        await _navigationService.NavigateToAsync("joinconvoy");
    }

    [RelayCommand]
    private async Task SelectConvoy(ConvoyDto convoy)
    {
        await _navigationService.NavigateToAsync("convoydetail", new Dictionary<string, string>
        {
            ["convoyId"] = convoy.Id.ToString(),
            ["joinCode"] = convoy.JoinCode
        });
    }

    [RelayCommand]
    private async Task LeaveConvoy(string joinCode)
    {
        try
        {
            var confirm = await _dialogService.ConfirmAsync(
                "Confirmation",
                "Voulez-vous vraiment quitter ce convoi ?");

            if (!confirm) return;

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

    public static string GetRoleName(int role)
    {
        return role switch
        {
            2 => "Leader",
            _ => "Membre"
        };
    }
}
