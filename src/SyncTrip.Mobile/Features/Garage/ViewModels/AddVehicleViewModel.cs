using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.Mobile.Core.Services;
using SyncTrip.Shared.DTOs.Brands;
using SyncTrip.Shared.DTOs.Vehicles;

namespace SyncTrip.Mobile.Features.Garage.ViewModels;

/// <summary>
/// ViewModel pour la page d'ajout d'un véhicule.
/// Permet de sélectionner une marque, saisir le modèle, type, couleur et année.
/// </summary>
public partial class AddVehicleViewModel : ObservableObject
{
    private readonly IVehicleService _vehicleService;
    private readonly IBrandService _brandService;

    /// <summary>
    /// Collection observable des marques disponibles.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<BrandDto> brands = new();

    /// <summary>
    /// Marque sélectionnée.
    /// </summary>
    [ObservableProperty]
    private BrandDto? selectedBrand;

    /// <summary>
    /// Modèle du véhicule.
    /// </summary>
    [ObservableProperty]
    private string model = string.Empty;

    /// <summary>
    /// Type de véhicule sélectionné (VehicleType enum : 1=Car, 2=Motorcycle, 3=Truck, 4=Van, 5=Motorhome).
    /// </summary>
    [ObservableProperty]
    private int selectedVehicleType = 1; // Par défaut : Car

    /// <summary>
    /// Couleur du véhicule (facultatif).
    /// </summary>
    [ObservableProperty]
    private string? color;

    /// <summary>
    /// Année de fabrication (facultatif).
    /// </summary>
    [ObservableProperty]
    private int? year;

    /// <summary>
    /// Indique si une opération de chargement est en cours.
    /// </summary>
    [ObservableProperty]
    private bool isLoading;

    /// <summary>
    /// Indique si une opération de sauvegarde est en cours.
    /// </summary>
    [ObservableProperty]
    private bool isSaving;

    /// <summary>
    /// Message d'erreur à afficher.
    /// </summary>
    [ObservableProperty]
    private string? errorMessage;

    /// <summary>
    /// Année minimale (1900).
    /// </summary>
    public int MinimumYear => 1900;

    /// <summary>
    /// Année maximale (année courante + 1).
    /// </summary>
    public int MaximumYear => DateTime.UtcNow.Year + 1;

    /// <summary>
    /// Types de véhicules disponibles avec leur nom affiché.
    /// </summary>
    public ObservableCollection<VehicleTypeItem> VehicleTypes { get; } = new()
    {
        new VehicleTypeItem { Value = 1, Name = "Voiture" },
        new VehicleTypeItem { Value = 2, Name = "Moto" },
        new VehicleTypeItem { Value = 3, Name = "Camion" },
        new VehicleTypeItem { Value = 4, Name = "Van" },
        new VehicleTypeItem { Value = 5, Name = "Camping-car" }
    };

    /// <summary>
    /// Initialise une nouvelle instance du ViewModel.
    /// </summary>
    /// <param name="vehicleService">Service véhicule.</param>
    /// <param name="brandService">Service marques.</param>
    public AddVehicleViewModel(IVehicleService vehicleService, IBrandService brandService)
    {
        _vehicleService = vehicleService;
        _brandService = brandService;
    }

    /// <summary>
    /// Charge la liste des marques disponibles.
    /// </summary>
    [RelayCommand]
    private async Task LoadBrands()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var brandList = await _brandService.GetBrandsAsync();

            Brands.Clear();
            foreach (var brand in brandList)
            {
                Brands.Add(brand);
            }

            // Sélectionner automatiquement la première marque si disponible
            if (Brands.Count > 0)
            {
                SelectedBrand = Brands[0];
            }
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

    /// <summary>
    /// Sauvegarde le nouveau véhicule.
    /// </summary>
    [RelayCommand]
    private async Task SaveVehicle()
    {
        // Validation client-side
        if (SelectedBrand == null)
        {
            ErrorMessage = "Veuillez sélectionner une marque.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Model))
        {
            ErrorMessage = "Le modèle est obligatoire.";
            return;
        }

        if (Model.Length > 100)
        {
            ErrorMessage = "Le modèle ne peut pas dépasser 100 caractères.";
            return;
        }

        if (Year.HasValue && (Year.Value < MinimumYear || Year.Value > MaximumYear))
        {
            ErrorMessage = $"L'année doit être comprise entre {MinimumYear} et {MaximumYear}.";
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
                // Navigation retour vers la liste des véhicules
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ErrorMessage = "Impossible de créer le véhicule.";
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

    /// <summary>
    /// Annule l'ajout et retourne à la page précédente.
    /// </summary>
    [RelayCommand]
    private async Task Cancel()
    {
        try
        {
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur de navigation: {ex.Message}";
        }
    }
}

/// <summary>
/// Item représentant un type de véhicule pour le Picker.
/// </summary>
public class VehicleTypeItem
{
    /// <summary>
    /// Valeur de l'enum VehicleType.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Nom affiché du type.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
