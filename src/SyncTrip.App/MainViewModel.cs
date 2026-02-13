using CommunityToolkit.Mvvm.ComponentModel;
using SyncTrip.App.Features.Profile.ViewModels;
using SyncTrip.App.Features.Garage.ViewModels;
using SyncTrip.App.Features.Convoy.ViewModels;

namespace SyncTrip.App;

public partial class MainViewModel : ObservableObject
{
    public ProfileViewModel ProfileViewModel { get; }
    public GarageViewModel GarageViewModel { get; }
    public ConvoyLobbyViewModel ConvoyLobbyViewModel { get; }

    [ObservableProperty]
    private int selectedTabIndex;

    public MainViewModel(ProfileViewModel profileViewModel, GarageViewModel garageViewModel, ConvoyLobbyViewModel convoyLobbyViewModel)
    {
        ProfileViewModel = profileViewModel;
        GarageViewModel = garageViewModel;
        ConvoyLobbyViewModel = convoyLobbyViewModel;
    }
}
