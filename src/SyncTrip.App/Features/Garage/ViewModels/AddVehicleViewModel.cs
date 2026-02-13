using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.App.Core.Platform;
using SyncTrip.App.Core.Services;
using SyncTrip.Shared.DTOs.Brands;
using SyncTrip.Shared.DTOs.Vehicles;

namespace SyncTrip.App.Features.Garage.ViewModels;

public partial class AddVehicleViewModel : ObservableObject
{
    private readonly IVehicleService _vehicleService;
    private readonly IBrandService _brandService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<BrandDto> brands = new();

    [ObservableProperty]
    private BrandDto? selectedBrand;

    [ObservableProperty]
    private string model = string.Empty;

    [ObservableProperty]
    private int selectedVehicleType = 1;

    [ObservableProperty]
    private string? color;

    [ObservableProperty]
    private int? year;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isSaving;

    [ObservableProperty]
    private string? errorMessage;

    public int MinimumYear => 1900;
    public int MaximumYear => DateTime.UtcNow.Year + 1;

    public ObservableCollection<VehicleTypeItem> VehicleTypes { get; } = new()
    {
        new VehicleTypeItem { Value = 1, Name = "Voiture" },
        new VehicleTypeItem { Value = 2, Name = "Moto" },
        new VehicleTypeItem { Value = 3, Name = "Camion" },
        new VehicleTypeItem { Value = 4, Name = "Van" },
        new VehicleTypeItem { Value = 5, Name = "Camping-car" }
    };

    public AddVehicleViewModel(IVehicleService vehicleService, IBrandService brandService, INavigationService navigationService)
    {
        _vehicleService = vehicleService;
        _brandService = brandService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    public async Task LoadBrands()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var brandList = await _brandService.GetBrandsAsync();

            Brands.Clear();
            foreach (var brand in brandList)
                Brands.Add(brand);

            if (Brands.Count > 0)
                SelectedBrand = Brands[0];
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors du chargement des marques: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveVehicle()
    {
        if (SelectedBrand == null)
        {
            ErrorMessage = "Veuillez selectionner une marque.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Model))
        {
            ErrorMessage = "Le modele est obligatoire.";
            return;
        }

        if (Model.Length > 100)
        {
            ErrorMessage = "Le modele ne peut pas depasser 100 caracteres.";
            return;
        }

        if (Year.HasValue && (Year.Value < MinimumYear || Year.Value > MaximumYear))
        {
            ErrorMessage = $"L'annee doit etre comprise entre {MinimumYear} et {MaximumYear}.";
            return;
        }

        try
        {
            IsSaving = true;
            ErrorMessage = null;

            var request = new CreateVehicleRequest
            {
                BrandId = SelectedBrand.Id,
                Model = Model.Trim(),
                Type = SelectedVehicleType,
                Color = string.IsNullOrWhiteSpace(Color) ? null : Color.Trim(),
                Year = Year
            };

            var vehicleId = await _vehicleService.CreateVehicleAsync(request);

            if (vehicleId.HasValue)
            {
                await _navigationService.GoBackAsync();
            }
            else
            {
                ErrorMessage = "Impossible de creer le vehicule.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await _navigationService.GoBackAsync();
    }
}

public class VehicleTypeItem
{
    public int Value { get; set; }
    public string Name { get; set; } = string.Empty;
}
