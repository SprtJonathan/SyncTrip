using SyncTrip.Mobile.Features.Authentication.Views;
using SyncTrip.Mobile.Features.Garage.Views;

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

		// Enregistrement des routes de navigation pour Feature 2
		Routing.RegisterRoute("addvehicle", typeof(AddVehiclePage));
	}
}
