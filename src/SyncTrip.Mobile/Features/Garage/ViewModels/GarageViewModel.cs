using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.Mobile.Core.Services;
using SyncTrip.Shared.DTOs.Vehicles;

namespace SyncTrip.Mobile.Features.Garage.ViewModels;

/// <summary>
/// ViewModel pour la page de liste des véhicules (Garage).
/// Affiche tous les véhicules de l'utilisateur avec possibilité de suppression et navigation vers ajout/édition.
/// </summary>
public partial class GarageViewModel : ObservableObject
{
    private readonly IVehicleService _vehicleService;

    /// <summary>
    /// Collection observable des véhicules.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<VehicleDto> vehicles = new();

    /// <summary>
    /// Indique si une opération de chargement est en cours.
    /// </summary>
    [ObservableProperty]
    private bool isLoading;

    /// <summary>
    /// Indique si une opération de suppression est en cours.
    /// </summary>
    [ObservableProperty]
    private bool isDeleting;

    /// <summary>
    /// Message d'erreur à afficher.
    /// </summary>
    [ObservableProperty]
    private string? errorMessage;

    /// <summary>
    /// Message de succès à afficher.
    /// </summary>
    [ObservableProperty]
    private string? successMessage;

    /// <summary>
    /// Indique si la liste est vide.
    /// </summary>
    [ObservableProperty]
    private bool isEmpty;

    /// <summary>
    /// Initialise une nouvelle instance du ViewModel.
    /// </summary>
    /// <param name="vehicleService">Service véhicule.</param>
    public GarageViewModel(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    /// <summary>
    /// Charge la liste des véhicules.
    /// </summary>
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

    /// <summary>
    /// Navigate vers la page d'ajout d'un nouveau véhicule.
    /// </summary>
    [RelayCommand]
    private async Task AddVehicle()
    {
        try
        {
            await Shell.Current.GoToAsync("addvehicle");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur de navigation: {ex.Message}";
        }
    }

    /// <summary>
    /// Supprime un véhicule après confirmation.
    /// </summary>
    /// <param name="vehicleId">Identifiant du véhicule à supprimer.</param>
    [RelayCommand]
    private async Task DeleteVehicle(Guid vehicleId)
    {
        try
        {
            // Confirmation de suppression
            var vehicle = Vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (vehicle == null)
                return;

            var confirm = await Shell.Current.DisplayAlert(
                "Confirmation",
                $"Voulez-vous vraiment supprimer {vehicle.BrandName} {vehicle.Model} ?",
                "Oui",
                "Non"
            );

            if (!confirm)
                return;

            IsDeleting = true;
            ErrorMessage = null;
            SuccessMessage = null;

            var success = await _vehicleService.DeleteVehicleAsync(vehicleId);

            if (success)
            {
                Vehicles.Remove(vehicle);
                IsEmpty = Vehicles.Count == 0;
                SuccessMessage = "Véhicule supprimé avec succès.";
            }
            else
            {
                ErrorMessage = "Impossible de supprimer le véhicule.";
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

    /// <summary>
    /// Rafraîchit la liste des véhicules.
    /// </summary>
    [RelayCommand]
    private async Task Refresh()
    {
        await LoadVehicles();
    }

    /// <summary>
    /// Récupère le nom du type de véhicule.
    /// </summary>
    /// <param name="typeValue">Valeur de l'enum VehicleType.</param>
    /// <returns>Nom du type en français.</returns>
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
