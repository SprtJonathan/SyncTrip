using Avalonia.Controls;
using SyncTrip.App.Features.Convoy.ViewModels;

namespace SyncTrip.App.Features.Convoy.Views;

public partial class ConvoyLobbyView : UserControl
{
    public ConvoyLobbyView()
    {
        InitializeComponent();
    }

    protected override async void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataContext is ConvoyLobbyViewModel vm)
            await vm.LoadConvoysCommand.ExecuteAsync(null);
    }
}
