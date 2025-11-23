using SyncTrip.Mobile.Features.Authentication.Views;

namespace SyncTrip.Mobile;

/// <summary>
/// Shell principal de l'application.
/// Gère la navigation et l'enregistrement des routes.
/// </summary>
public partial class AppShell : Shell
{
	/// <summary>
	/// Initialise le Shell et enregistre les routes de navigation.
	/// </summary>
	public AppShell()
	{
		InitializeComponent();

		// Enregistrement des routes de navigation pour l'authentification
		Routing.RegisterRoute("magic-link", typeof(MagicLinkPage));
		Routing.RegisterRoute("registration", typeof(RegistrationPage));
	}
}
