namespace SyncTrip.App.Core.Platform;

public interface INavigationService
{
    Task NavigateToAsync(string route, Dictionary<string, string>? parameters = null);
    Task GoBackAsync();
}
