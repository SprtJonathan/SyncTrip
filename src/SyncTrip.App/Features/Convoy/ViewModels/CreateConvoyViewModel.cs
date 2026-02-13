using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.App.Core.Platform;
using SyncTrip.App.Core.Services;
using SyncTrip.Shared.DTOs.Convoys;
using SyncTrip.Shared.DTOs.Vehicles;

namespace SyncTrip.App.Features.Convoy.ViewModels;

public partial class CreateConvoyViewModel : ObservableObject
{
    private readonly IConvoyService _convoyService;
    private readonly IVehicleService _vehicleService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<VehicleDto> vehicles = new();

    [ObservableProperty]
    private VehicleDto? selectedVehicle;

    [ObservableProperty]
    private bool isPrivate;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool hasVehicles;

    public CreateConvoyViewModel(IConvoyService convoyService, IVehicleService vehicleService, INavigationService navigationService)
    {
        _convoyService = convoyService;
        _vehicleService = vehicleService;
        _navigationService = navigationService;
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
    private async Task CreateConvoy()
    {
        if (SelectedVehicle == null)
        {
            ErrorMessage = "Veuillez selectionner un vehicule.";
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var request = new CreateConvoyRequest
            {
                VehicleId = SelectedVehicle.Id,
                IsPrivate = IsPrivate
            };

            var convoyId = await _convoyService.CreateConvoyAsync(request);

            if (convoyId.HasValue)
            {
                await _navigationService.GoBackAsync();
            }
            else
            {
                ErrorMessage = "Impossible de creer le convoi.";
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
