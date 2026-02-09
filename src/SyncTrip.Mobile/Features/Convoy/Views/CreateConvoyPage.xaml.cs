using SyncTrip.Mobile.Features.Convoy.ViewModels;

namespace SyncTrip.Mobile.Features.Convoy.Views;

/// <summary>
/// Page de creation d'un nouveau convoi.
/// </summary>
public partial class CreateConvoyPage : ContentPage
{
    private readonly CreateConvoyViewModel _viewModel;

    public CreateConvoyPage(CreateConvoyViewModel viewModel)
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
