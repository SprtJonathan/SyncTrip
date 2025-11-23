using SyncTrip.Mobile.Features.Authentication.ViewModels;

namespace SyncTrip.Mobile.Features.Authentication.Views;

/// <summary>
/// Page pour l'envoi du Magic Link.
/// Permet à l'utilisateur de saisir son email pour recevoir un lien de connexion.
/// </summary>
public partial class MagicLinkPage : ContentPage
{
    /// <summary>
    /// Initialise une nouvelle instance de la page.
    /// </summary>
    /// <param name="viewModel">ViewModel injecté via DI.</param>
    public MagicLinkPage(MagicLinkViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
