using SyncTrip.Mobile.Features.Convoy.ViewModels;

namespace SyncTrip.Mobile.Features.Convoy.Views;

/// <summary>
/// Page de liste des convois de l'utilisateur.
/// </summary>
public partial class ConvoyLobbyPage : ContentPage
{
    private readonly ConvoyLobbyViewModel _viewModel;

    public ConvoyLobbyPage(ConvoyLobbyViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadConvoysCommand.ExecuteAsync(null);
    }
}
