using SyncTrip.Mobile.Features.Authentication.ViewModels;

namespace SyncTrip.Mobile.Features.Authentication.Views;

/// <summary>
/// Page pour l'inscription d'un nouvel utilisateur.
/// Permet de saisir les informations requises (pseudo, date de naissance) et optionnelles (nom, prénom).
/// </summary>
public partial class RegistrationPage : ContentPage
{
    private readonly RegistrationViewModel _viewModel;

    /// <summary>
    /// Initialise une nouvelle instance de la page.
    /// </summary>
    /// <param name="viewModel">ViewModel injecté via DI.</param>
    public RegistrationPage(RegistrationViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <summary>
    /// Appelé lorsque la page apparaît.
    /// Peut être utilisé pour initialiser le ViewModel avec l'email depuis la navigation.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        // L'email sera passé via QueryProperty ou directement dans le ViewModel
    }
}
