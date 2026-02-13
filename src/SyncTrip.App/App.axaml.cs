using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using SyncTrip.App.Core.Http;
using SyncTrip.App.Core.Platform;
using SyncTrip.App.Core.Services;
using SyncTrip.App.Features.Authentication.ViewModels;
using SyncTrip.App.Features.Convoy.ViewModels;
using SyncTrip.App.Features.Garage.ViewModels;
using SyncTrip.App.Features.Profile.ViewModels;
using SyncTrip.App.Features.Trip.ViewModels;
using SyncTrip.App.Navigation;

namespace SyncTrip.App;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var navigationService = (NavigationService)Services.GetRequiredService<INavigationService>();
            RegisterRoutes(navigationService);

            var mainWindow = new MainWindow
            {
                DataContext = navigationService
            };
            desktop.MainWindow = mainWindow;

            // Auth check au demarrage
            _ = CheckAuthenticationAsync(navigationService);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Platform services
        services.AddSingleton<ISecureStorageService, DesktopSecureStorageService>();
        services.AddSingleton<ILocationService, DesktopLocationService>();
        services.AddSingleton<IDialogService, DialogService>();

        // Navigation
        services.AddSingleton<INavigationService, NavigationService>();

        // HTTP
        services.AddTransient<AuthorizationMessageHandler>();
        services.AddHttpClient<IApiService, ApiService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:5001");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<AuthorizationMessageHandler>();

        // Services
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IVehicleService, VehicleService>();
        services.AddSingleton<IBrandService, BrandService>();
        services.AddSingleton<IConvoyService, ConvoyService>();
        services.AddSingleton<ITripService, TripService>();
        services.AddSingleton<ISignalRService, SignalRService>();

        // ViewModels
        services.AddTransient<MagicLinkViewModel>();
        services.AddTransient<RegistrationViewModel>();
        services.AddTransient<ProfileViewModel>();
        services.AddTransient<GarageViewModel>();
        services.AddTransient<AddVehicleViewModel>();
        services.AddTransient<ConvoyLobbyViewModel>();
        services.AddTransient<CreateConvoyViewModel>();
        services.AddTransient<JoinConvoyViewModel>();
        services.AddTransient<ConvoyDetailViewModel>();
        services.AddTransient<CockpitViewModel>();
        services.AddTransient<MainViewModel>();
    }

    private static void RegisterRoutes(NavigationService nav)
    {
        nav.RegisterRoute("login", typeof(MagicLinkViewModel));
        nav.RegisterRoute("registration", typeof(RegistrationViewModel));
        nav.RegisterRoute("main", typeof(MainViewModel));
        nav.RegisterRoute("addvehicle", typeof(AddVehicleViewModel));
        nav.RegisterRoute("createconvoy", typeof(CreateConvoyViewModel));
        nav.RegisterRoute("joinconvoy", typeof(JoinConvoyViewModel));
        nav.RegisterRoute("convoydetail", typeof(ConvoyDetailViewModel));
        nav.RegisterRoute("cockpit", typeof(CockpitViewModel));
    }

    private static async Task CheckAuthenticationAsync(NavigationService nav)
    {
        var authService = Services.GetRequiredService<IAuthenticationService>();
        var isAuthenticated = await authService.IsAuthenticatedAsync();

        if (isAuthenticated)
            await nav.NavigateToAsync("main");
        else
            await nav.NavigateToAsync("login");
    }
}
