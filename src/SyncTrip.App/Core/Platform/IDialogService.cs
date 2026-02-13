namespace SyncTrip.App.Core.Platform;

public interface IDialogService
{
    Task<bool> ConfirmAsync(string title, string message, string accept = "Oui", string cancel = "Non");
    Task AlertAsync(string title, string message, string close = "OK");
}
