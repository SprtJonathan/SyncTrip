using Avalonia.Controls;
using SyncTrip.App.Features.Convoy.ViewModels;

namespace SyncTrip.App.Features.Convoy.Views;

public partial class ConvoyDetailView : UserControl
{
    public ConvoyDetailView()
    {
        InitializeComponent();
    }

    protected override async void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataContext is ConvoyDetailViewModel vm)
            await vm.LoadDetailsCommand.ExecuteAsync(null);
    }
}
