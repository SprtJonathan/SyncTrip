using Microsoft.Extensions.Logging;
using SyncTrip.Mobile.Core.Services;
using SyncTrip.Mobile.Features.Authentication.ViewModels;
using SyncTrip.Mobile.Features.Authentication.Views;
using SyncTrip.Mobile.Features.Profile.ViewModels;
using SyncTrip.Mobile.Features.Profile.Views;
using SyncTrip.Mobile.Features.Garage.ViewModels;
using SyncTrip.Mobile.Features.Garage.Views;

namespace SyncTrip.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Configuration HttpClient et ApiService
		builder.Services.AddHttpClient<IApiService, ApiService>(client =>
		{
			// URL de base pour le développement (à configurer selon l'environnement)
			client.BaseAddress = new Uri("https://localhost:5001");
			client.Timeout = TimeSpan.FromSeconds(30);
		});

		// Services
		builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
		builder.Services.AddSingleton<IUserService, UserService>();
		builder.Services.AddSingleton<IVehicleService, VehicleService>();
		builder.Services.AddSingleton<IBrandService, BrandService>();

		// ViewModels - Authentication
		builder.Services.AddTransient<MagicLinkViewModel>();
		builder.Services.AddTransient<RegistrationViewModel>();

		// ViewModels - Profile & Garage
		builder.Services.AddTransient<ProfileViewModel>();
		builder.Services.AddTransient<GarageViewModel>();
		builder.Services.AddTransient<AddVehicleViewModel>();

		// Pages - Authentication
		builder.Services.AddTransient<MagicLinkPage>();
		builder.Services.AddTransient<RegistrationPage>();

		// Pages - Profile & Garage
		builder.Services.AddTransient<ProfilePage>();
		builder.Services.AddTransient<GaragePage>();
		builder.Services.AddTransient<AddVehiclePage>();

		return builder.Build();
	}
}
