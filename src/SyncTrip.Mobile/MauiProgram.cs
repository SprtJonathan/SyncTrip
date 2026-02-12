using Microsoft.Extensions.Logging;
using SyncTrip.Mobile.Core.Http;
using SyncTrip.Mobile.Core.Services;
using SyncTrip.Mobile.Features.Authentication.ViewModels;
using SyncTrip.Mobile.Features.Authentication.Views;
using SyncTrip.Mobile.Features.Profile.ViewModels;
using SyncTrip.Mobile.Features.Profile.Views;
using SyncTrip.Mobile.Features.Garage.ViewModels;
using SyncTrip.Mobile.Features.Garage.Views;
using SyncTrip.Mobile.Features.Convoy.ViewModels;
using SyncTrip.Mobile.Features.Convoy.Views;

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
		builder.Services.AddTransient<AuthorizationMessageHandler>();
		builder.Services.AddHttpClient<IApiService, ApiService>(client =>
		{
			// URL de base pour le développement (à configurer selon l'environnement)
			client.BaseAddress = new Uri("https://localhost:5001");
			client.Timeout = TimeSpan.FromSeconds(30);
		})
		.AddHttpMessageHandler<AuthorizationMessageHandler>();

		// Services
		builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
		builder.Services.AddSingleton<IUserService, UserService>();
		builder.Services.AddSingleton<IVehicleService, VehicleService>();
		builder.Services.AddSingleton<IBrandService, BrandService>();
		builder.Services.AddSingleton<IConvoyService, ConvoyService>();

		// ViewModels - Authentication
		builder.Services.AddTransient<MagicLinkViewModel>();
		builder.Services.AddTransient<RegistrationViewModel>();

		// ViewModels - Profile & Garage
		builder.Services.AddTransient<ProfileViewModel>();
		builder.Services.AddTransient<GarageViewModel>();
		builder.Services.AddTransient<AddVehicleViewModel>();

		// ViewModels - Convoy
		builder.Services.AddTransient<ConvoyLobbyViewModel>();
		builder.Services.AddTransient<CreateConvoyViewModel>();
		builder.Services.AddTransient<JoinConvoyViewModel>();

		// Pages - Authentication
		builder.Services.AddTransient<MagicLinkPage>();
		builder.Services.AddTransient<RegistrationPage>();

		// Pages - Profile & Garage
		builder.Services.AddTransient<ProfilePage>();
		builder.Services.AddTransient<GaragePage>();
		builder.Services.AddTransient<AddVehiclePage>();

		// Pages - Convoy
		builder.Services.AddTransient<ConvoyLobbyPage>();
		builder.Services.AddTransient<CreateConvoyPage>();
		builder.Services.AddTransient<JoinConvoyPage>();

		return builder.Build();
	}
}
