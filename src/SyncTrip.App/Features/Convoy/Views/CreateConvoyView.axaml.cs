using Avalonia.Controls;
using SyncTrip.App.Features.Convoy.ViewModels;

namespace SyncTrip.App.Features.Convoy.Views;

public partial class CreateConvoyView : UserControl
{
    public CreateConvoyView()
    {
        InitializeComponent();
    }

    protected override async void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataContext is CreateConvoyViewModel vm)
            await vm.LoadVehiclesCommand.ExecuteAsync(null);
    }
}
