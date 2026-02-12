using SyncTrip.Mobile.Core.Services;

namespace SyncTrip.Mobile;

public partial class App : Application
{
	private readonly IAuthenticationService _authService;

	public App(IAuthenticationService authService)
	{
		InitializeComponent();
		_authService = authService;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var shell = new AppShell();
		var window = new Window(shell);

		shell.Navigated += async (s, e) =>
		{
			// Vérifier l'authentification une seule fois au démarrage
			shell.Navigated -= (s, e) => { };
			var isAuthenticated = await _authService.IsAuthenticatedAsync();
			if (isAuthenticated)
			{
				await shell.GoToAsync("//main");
			}
			// Sinon, reste sur la page login (route par défaut)
		};

		return window;
	}
}
