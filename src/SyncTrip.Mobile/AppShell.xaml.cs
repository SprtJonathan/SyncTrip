using SyncTrip.Mobile.Features.Authentication.Views;
using SyncTrip.Mobile.Features.Convoy.Views;
using SyncTrip.Mobile.Features.Garage.Views;
using SyncTrip.Mobile.Features.Trip.Views;

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

		// Routes d'authentification
		Routing.RegisterRoute("registration", typeof(RegistrationPage));

		// Routes Garage
		Routing.RegisterRoute("addvehicle", typeof(AddVehiclePage));

		// Routes Convois
		Routing.RegisterRoute("createconvoy", typeof(CreateConvoyPage));
		Routing.RegisterRoute("joinconvoy", typeof(JoinConvoyPage));
		Routing.RegisterRoute("convoydetail", typeof(ConvoyDetailPage));

		// Routes Trip
		Routing.RegisterRoute("cockpit", typeof(CockpitPage));
	}
}
