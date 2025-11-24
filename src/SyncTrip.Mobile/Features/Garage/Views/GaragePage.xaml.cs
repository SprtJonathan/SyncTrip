using SyncTrip.Mobile.Features.Garage.ViewModels;

namespace SyncTrip.Mobile.Features.Garage.Views;

/// <summary>
/// Page d'affichage de la liste des véhicules (Garage).
/// Permet de visualiser, ajouter et supprimer des véhicules.
/// </summary>
public partial class GaragePage : ContentPage
{
    private readonly GarageViewModel _viewModel;

    /// <summary>
    /// Initialise une nouvelle instance de la page Garage.
    /// </summary>
    /// <param name="viewModel">ViewModel de la page Garage.</param>
    public GaragePage(GarageViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <summary>
    /// Gère l'apparition de la page et charge les véhicules.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadVehiclesCommand.ExecuteAsync(null);
    }
}
