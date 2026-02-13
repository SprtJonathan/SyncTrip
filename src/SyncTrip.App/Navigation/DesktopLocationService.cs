using SyncTrip.App.Core.Platform;

namespace SyncTrip.App.Navigation;

public class DesktopLocationService : ILocationService
{
    public Task<LocationResult?> GetCurrentLocationAsync()
    {
        // Desktop n'a pas de GPS - retourne null
        return Task.FromResult<LocationResult?>(null);
    }
}
