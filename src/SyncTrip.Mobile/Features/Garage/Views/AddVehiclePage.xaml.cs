using SyncTrip.Mobile.Features.Garage.ViewModels;

namespace SyncTrip.Mobile.Features.Garage.Views;

/// <summary>
/// Page d'ajout d'un nouveau véhicule au garage.
/// Permet de saisir les informations du véhicule (marque, modèle, type, couleur, année).
/// </summary>
public partial class AddVehiclePage : ContentPage
{
    private readonly AddVehicleViewModel _viewModel;

    /// <summary>
    /// Initialise une nouvelle instance de la page AddVehicle.
    /// </summary>
    /// <param name="viewModel">ViewModel de la page d'ajout de véhicule.</param>
    public AddVehiclePage(AddVehicleViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <summary>
    /// Gère l'apparition de la page et charge les marques.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadBrandsCommand.ExecuteAsync(null);
    }
}
