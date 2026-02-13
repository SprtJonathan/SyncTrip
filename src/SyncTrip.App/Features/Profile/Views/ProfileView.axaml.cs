using Avalonia.Controls;
using SyncTrip.App.Features.Profile.ViewModels;

namespace SyncTrip.App.Features.Profile.Views;

public partial class ProfileView : UserControl
{
    public ProfileView()
    {
        InitializeComponent();
    }

    protected override async void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataContext is ProfileViewModel vm)
            await vm.LoadProfileCommand.ExecuteAsync(null);
    }
}
