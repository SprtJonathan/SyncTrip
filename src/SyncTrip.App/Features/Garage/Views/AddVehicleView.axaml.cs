using Avalonia.Controls;
using SyncTrip.App.Features.Garage.ViewModels;

namespace SyncTrip.App.Features.Garage.Views;

public partial class AddVehicleView : UserControl
{
    public AddVehicleView()
    {
        InitializeComponent();
    }

    protected override async void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataContext is AddVehicleViewModel vm)
            await vm.LoadBrandsCommand.ExecuteAsync(null);
    }
}
