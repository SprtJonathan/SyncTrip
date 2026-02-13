using Avalonia.Controls;
using SyncTrip.App.Features.Garage.ViewModels;

namespace SyncTrip.App.Features.Garage.Views;

public partial class GarageView : UserControl
{
    public GarageView()
    {
        InitializeComponent();
    }

    protected override async void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataContext is GarageViewModel vm)
            await vm.LoadVehiclesCommand.ExecuteAsync(null);
    }
}
