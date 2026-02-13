using Avalonia.Controls;
using SyncTrip.App.Features.Chat.ViewModels;

namespace SyncTrip.App.Features.Chat.Views;

public partial class ChatView : UserControl
{
    public ChatView()
    {
        InitializeComponent();
    }

    protected override async void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataContext is ChatViewModel vm)
            await vm.LoadMessagesCommand.ExecuteAsync(null);
    }

    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is ChatViewModel vm)
            _ = vm.Cleanup();

        base.OnDetachedFromVisualTree(e);
    }
}
