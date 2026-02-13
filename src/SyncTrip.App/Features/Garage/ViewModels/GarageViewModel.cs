using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.App.Core.Platform;
using SyncTrip.App.Core.Services;
using SyncTrip.Shared.DTOs.Vehicles;

namespace SyncTrip.App.Features.Garage.ViewModels;

public partial class GarageViewModel : ObservableObject
{
    private readonly IVehicleService _vehicleService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<VehicleDto> vehicles = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isDeleting;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? successMessage;

    [ObservableProperty]
    private bool isEmpty;

    public GarageViewModel(IVehicleService vehicleService, INavigationService navigationService, IDialogService dialogService)
    {
        _vehicleService = vehicleService;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    public async Task LoadVehicles()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var vehicleList = await _vehicleService.GetVehiclesAsync();

            Vehicles.Clear();
            foreach (var vehicle in vehicleList)
                Vehicles.Add(vehicle);

            IsEmpty = Vehicles.Count == 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors du chargement: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddVehicle()
    {
        try
        {
            await _navigationService.NavigateToAsync("addvehicle");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur de navigation: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DeleteVehicle(Guid vehicleId)
    {
        try
        {
            var vehicle = Vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (vehicle == null) return;

            var confirm = await _dialogService.ConfirmAsync(
                "Confirmation",
                $"Voulez-vous vraiment supprimer {vehicle.BrandName} {vehicle.Model} ?");

            if (!confirm) return;

            IsDeleting = true;
            ErrorMessage = null;
            SuccessMessage = null;

            var success = await _vehicleService.DeleteVehicleAsync(vehicleId);

            if (success)
            {
                Vehicles.Remove(vehicle);
                IsEmpty = Vehicles.Count == 0;
                SuccessMessage = "Vehicule supprime avec succes.";
            }
            else
            {
                ErrorMessage = "Impossible de supprimer le vehicule.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
        finally
        {
            IsDeleting = false;
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadVehicles();
    }

    public static string GetVehicleTypeName(int typeValue)
    {
        return typeValue switch
        {
            1 => "Voiture",
            2 => "Moto",
            3 => "Camion",
            4 => "Van",
            5 => "Camping-car",
            _ => "Inconnu"
        };
    }
}
