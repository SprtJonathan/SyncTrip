using Avalonia.Controls;
using Avalonia.Interactivity;
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

    private void OnRouteProfileFast(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ConvoyDetailViewModel vm)
            vm.SelectedRouteProfile = 1;
    }

    private void OnRouteProfileScenic(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ConvoyDetailViewModel vm)
            vm.SelectedRouteProfile = 2;
    }
}
