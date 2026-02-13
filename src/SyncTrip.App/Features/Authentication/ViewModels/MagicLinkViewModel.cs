using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.App.Core.Platform;
using SyncTrip.App.Core.Services;

namespace SyncTrip.App.Features.Authentication.ViewModels;

public partial class MagicLinkViewModel : ObservableObject
{
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string token = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? message;

    [ObservableProperty]
    private bool isSuccess;

    [ObservableProperty]
    private bool showTokenStep;

    public MagicLinkViewModel(IAuthenticationService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
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
                Message = "Un lien de connexion a ete envoye a votre email.";
                IsSuccess = true;
                ShowTokenStep = true;
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

    [RelayCommand]
    private async Task VerifyToken()
    {
        if (string.IsNullOrWhiteSpace(Token))
        {
            Message = "Veuillez coller le token recu par email";
            IsSuccess = false;
            return;
        }

        try
        {
            IsBusy = true;
            Message = null;

            var response = await _authService.VerifyTokenAsync(Token.Trim());

            if (response is null)
            {
                Message = "Token invalide ou expire. Veuillez reessayer.";
                IsSuccess = false;
                return;
            }

            if (response.RequiresRegistration)
            {
                Message = "Compte verifie ! Redirection vers l'inscription...";
                IsSuccess = true;
                await _navigationService.NavigateToAsync("registration", new Dictionary<string, string>
                {
                    ["email"] = Email
                });
            }
            else if (!string.IsNullOrEmpty(response.JwtToken))
            {
                await _authService.SaveTokenAsync(response.JwtToken);
                Message = "Connexion reussie !";
                IsSuccess = true;
                await _navigationService.NavigateToAsync("main");
            }
            else
            {
                Message = "Erreur de verification. Veuillez reessayer.";
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
