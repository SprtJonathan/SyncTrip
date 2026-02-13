using SyncTrip.Mobile.Features.Convoy.ViewModels;

namespace SyncTrip.Mobile.Features.Convoy.Views;

/// <summary>
/// Page de détail d'un convoi avec gestion des voyages.
/// </summary>
public partial class ConvoyDetailPage : ContentPage
{
    private readonly ConvoyDetailViewModel _viewModel;

    public ConvoyDetailPage(ConvoyDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDetailsCommand.ExecuteAsync(null);
    }
}
