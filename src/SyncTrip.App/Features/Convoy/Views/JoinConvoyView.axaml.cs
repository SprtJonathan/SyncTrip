using Avalonia.Controls;
using SyncTrip.App.Features.Convoy.ViewModels;

namespace SyncTrip.App.Features.Convoy.Views;

public partial class JoinConvoyView : UserControl
{
    public JoinConvoyView()
    {
        InitializeComponent();
    }

    protected override async void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataContext is JoinConvoyViewModel vm)
            await vm.LoadVehiclesCommand.ExecuteAsync(null);
    }
}
