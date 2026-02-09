using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.Mobile.Core.Services;
using SyncTrip.Shared.DTOs.Convoys;
using SyncTrip.Shared.DTOs.Vehicles;

namespace SyncTrip.Mobile.Features.Convoy.ViewModels;

/// <summary>
/// ViewModel pour la page de rejoindre un convoi.
/// </summary>
public partial class JoinConvoyViewModel : ObservableObject
{
    private readonly IConvoyService _convoyService;
    private readonly IVehicleService _vehicleService;

    [ObservableProperty]
    private string joinCode = string.Empty;

    [ObservableProperty]
    private ObservableCollection<VehicleDto> vehicles = new();

    [ObservableProperty]
    private VehicleDto? selectedVehicle;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? successMessage;

    [ObservableProperty]
    private bool hasVehicles;

    public JoinConvoyViewModel(IConvoyService convoyService, IVehicleService vehicleService)
    {
        _convoyService = convoyService;
        _vehicleService = vehicleService;
    }

    [RelayCommand]
    private async Task LoadVehicles()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var vehicleList = await _vehicleService.GetVehiclesAsync();

            Vehicles.Clear();
            foreach (var vehicle in vehicleList)
            {
                Vehicles.Add(vehicle);
            }

            HasVehicles = Vehicles.Count > 0;

            if (!HasVehicles)
                ErrorMessage = "Ajoutez d'abord un vehicule dans votre garage.";
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
    private async Task JoinConvoy()
    {
        if (string.IsNullOrWhiteSpace(JoinCode) || JoinCode.Length != 6)
        {
            ErrorMessage = "Le code convoi doit contenir 6 caracteres.";
            return;
        }

        if (SelectedVehicle == null)
        {
            ErrorMessage = "Veuillez selectionner un vehicule.";
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = null;
            SuccessMessage = null;

            var request = new JoinConvoyRequest { VehicleId = SelectedVehicle.Id };
            var success = await _convoyService.JoinConvoyAsync(JoinCode.ToUpperInvariant(), request);

            if (success)
            {
                SuccessMessage = "Vous avez rejoint le convoi !";
                await Shell.Current.GoToAsync($"..?refresh=true");
            }
            else
            {
                ErrorMessage = "Impossible de rejoindre le convoi. Verifiez le code.";
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
}
