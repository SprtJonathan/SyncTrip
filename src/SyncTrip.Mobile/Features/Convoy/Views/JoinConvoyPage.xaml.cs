using SyncTrip.Mobile.Features.Convoy.ViewModels;

namespace SyncTrip.Mobile.Features.Convoy.Views;

/// <summary>
/// Page pour rejoindre un convoi existant via un code.
/// </summary>
public partial class JoinConvoyPage : ContentPage
{
    private readonly JoinConvoyViewModel _viewModel;

    public JoinConvoyPage(JoinConvoyViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadVehiclesCommand.ExecuteAsync(null);
    }
}
