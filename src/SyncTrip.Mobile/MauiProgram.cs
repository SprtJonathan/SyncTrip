using Microsoft.Extensions.Logging;
using SyncTrip.Mobile.Core.Services;
using SyncTrip.Mobile.Features.Authentication.ViewModels;
using SyncTrip.Mobile.Features.Authentication.Views;

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

		// ViewModels
		builder.Services.AddTransient<MagicLinkViewModel>();
		builder.Services.AddTransient<RegistrationViewModel>();

		// Pages
		builder.Services.AddTransient<MagicLinkPage>();
		builder.Services.AddTransient<RegistrationPage>();

		return builder.Build();
	}
}
