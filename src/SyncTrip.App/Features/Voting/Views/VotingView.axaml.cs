using Avalonia.Controls;
using SyncTrip.App.Features.Voting.ViewModels;

namespace SyncTrip.App.Features.Voting.Views;

public partial class VotingView : UserControl
{
    public VotingView()
    {
        InitializeComponent();
    }

    protected override async void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataContext is VotingViewModel vm)
        {
            vm.SubscribeToSignalR();
            await vm.LoadActiveProposalCommand.ExecuteAsync(null);
        }
    }

    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is VotingViewModel vm)
            vm.UnsubscribeFromSignalR();

        base.OnDetachedFromVisualTree(e);
    }
}
