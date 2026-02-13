using Avalonia.Controls;

namespace SyncTrip.App;

public partial class MainView : UserControl
{
    private bool _initialized;

    public MainView()
    {
        InitializeComponent();
    }

    private async void OnTabSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_initialized)
        {
            _initialized = true;
            return;
        }

        if (DataContext is not MainViewModel vm) return;
        if (sender is not TabControl tab) return;

        switch (tab.SelectedIndex)
        {
            case 0:
                await vm.ProfileViewModel.LoadProfileCommand.ExecuteAsync(null);
                break;
            case 1:
                await vm.GarageViewModel.LoadVehiclesCommand.ExecuteAsync(null);
                break;
            case 2:
                await vm.ConvoyLobbyViewModel.LoadConvoysCommand.ExecuteAsync(null);
                break;
        }
    }
}
