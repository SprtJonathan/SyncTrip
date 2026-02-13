using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using SyncTrip.App.Core.Platform;

namespace SyncTrip.App.Navigation;

public class DialogService : IDialogService
{
    public async Task<bool> ConfirmAsync(string title, string message, string accept = "Oui", string cancel = "Non")
    {
        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var result = false;

        var acceptButton = new Button { Content = accept, HorizontalAlignment = HorizontalAlignment.Right };
        var cancelButton = new Button { Content = cancel, HorizontalAlignment = HorizontalAlignment.Right };

        acceptButton.Click += (_, _) => { result = true; dialog.Close(); };
        cancelButton.Click += (_, _) => { result = false; dialog.Close(); };

        dialog.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Spacing = 15,
            Children =
            {
                new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Spacing = 10,
                    Children = { cancelButton, acceptButton }
                }
            }
        };

        var mainWindow = GetMainWindow();
        if (mainWindow != null)
            await dialog.ShowDialog(mainWindow);

        return result;
    }

    public async Task AlertAsync(string title, string message, string close = "OK")
    {
        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 180,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var closeButton = new Button { Content = close, HorizontalAlignment = HorizontalAlignment.Right };
        closeButton.Click += (_, _) => dialog.Close();

        dialog.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Spacing = 15,
            Children =
            {
                new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                closeButton
            }
        };

        var mainWindow = GetMainWindow();
        if (mainWindow != null)
            await dialog.ShowDialog(mainWindow);
    }

    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        return null;
    }
}
