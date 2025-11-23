using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.Mobile.Core.Services;

namespace SyncTrip.Mobile.Features.Authentication.ViewModels;

/// <summary>
/// ViewModel pour la page d'envoi de Magic Link.
/// Gère la validation de l'email et l'envoi de la requête au backend.
/// </summary>
public partial class MagicLinkViewModel : ObservableObject
{
    private readonly IAuthenticationService _authService;

    /// <summary>
    /// Adresse email saisie par l'utilisateur.
    /// </summary>
    [ObservableProperty]
    private string email = string.Empty;

    /// <summary>
    /// Indique si une opération est en cours.
    /// </summary>
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// Message à afficher à l'utilisateur (succès ou erreur).
    /// </summary>
    [ObservableProperty]
    private string? message;

    /// <summary>
    /// Indique si le message est un message de succès (pour le style).
    /// </summary>
    [ObservableProperty]
    private bool isSuccess;

    /// <summary>
    /// Initialise une nouvelle instance du ViewModel.
    /// </summary>
    /// <param name="authService">Service d'authentification.</param>
    public MagicLinkViewModel(IAuthenticationService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Commande pour envoyer le Magic Link.
    /// Valide l'email et effectue la requête au backend.
    /// </summary>
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
                Message = "Une erreur est survenue. Veuillez réessayer.";
                IsSuccess = false;
            }
        }
        catch
        {
            Message = "Une erreur est survenue. Veuillez réessayer.";
            IsSuccess = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Valide le format de l'adresse email.
    /// </summary>
    /// <param name="email">Email à valider.</param>
    /// <returns>True si l'email est valide, False sinon.</returns>
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
