using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.App.Core.Services;

namespace SyncTrip.App.Features.Authentication.ViewModels;

public partial class MagicLinkViewModel : ObservableObject
{
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? message;

    [ObservableProperty]
    private bool isSuccess;

    public MagicLinkViewModel(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task SendMagicLink()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            Message = "Veuillez entrer votre email";
            IsSuccess = false;
            return;
        }

        if (!IsValidEmail(Email))
        {
            Message = "Format d'email invalide";
            IsSuccess = false;
            return;
        }

        try
        {
            IsBusy = true;
            Message = null;

            var success = await _authService.SendMagicLinkAsync(Email);

            if (success)
            {
                Message = "Si un compte existe avec cet email, vous recevrez un lien de connexion.";
                IsSuccess = true;
            }
            else
            {
                Message = "Une erreur est survenue. Veuillez reessayer.";
                IsSuccess = false;
            }
        }
        catch
        {
            Message = "Une erreur est survenue. Veuillez reessayer.";
            IsSuccess = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
