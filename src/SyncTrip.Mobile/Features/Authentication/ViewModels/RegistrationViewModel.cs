using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.Mobile.Core.Services;
using SyncTrip.Shared.DTOs.Auth;

namespace SyncTrip.Mobile.Features.Authentication.ViewModels;

/// <summary>
/// ViewModel pour la page d'inscription.
/// Gère la validation des données utilisateur (notamment l'âge > 14 ans) et l'envoi au backend.
/// </summary>
public partial class RegistrationViewModel : ObservableObject
{
    private readonly IAuthenticationService _authService;

    /// <summary>
    /// Adresse email de l'utilisateur (transmise depuis la vérification du Magic Link).
    /// </summary>
    [ObservableProperty]
    private string email = string.Empty;

    /// <summary>
    /// Nom d'utilisateur (pseudo) - obligatoire.
    /// </summary>
    [ObservableProperty]
    private string username = string.Empty;

    /// <summary>
    /// Prénom de l'utilisateur - facultatif.
    /// </summary>
    [ObservableProperty]
    private string? firstName;

    /// <summary>
    /// Nom de famille de l'utilisateur - facultatif.
    /// </summary>
    [ObservableProperty]
    private string? lastName;

    /// <summary>
    /// Date de naissance de l'utilisateur - obligatoire.
    /// Initialisée à 15 ans en arrière pour respecter la contrainte > 14 ans.
    /// </summary>
    [ObservableProperty]
    private DateTime birthDate = DateTime.Now.AddYears(-15);

    /// <summary>
    /// Indique si une opération est en cours.
    /// </summary>
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// Message d'erreur à afficher à l'utilisateur.
    /// </summary>
    [ObservableProperty]
    private string? errorMessage;

    /// <summary>
    /// Date maximale pour le DatePicker (14 ans en arrière).
    /// L'utilisateur doit avoir au moins 14 ans.
    /// </summary>
    public DateTime MaximumDate => DateTime.Now.AddYears(-14);

    /// <summary>
    /// Initialise une nouvelle instance du ViewModel.
    /// </summary>
    /// <param name="authService">Service d'authentification.</param>
    public RegistrationViewModel(IAuthenticationService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Initialise le ViewModel avec l'email de l'utilisateur.
    /// </summary>
    /// <param name="email">Adresse email.</param>
    public void Initialize(string email)
    {
        Email = email;
    }

    /// <summary>
    /// Commande pour compléter l'inscription.
    /// Valide toutes les données avant d'envoyer au backend.
    /// </summary>
    [RelayCommand]
    private async Task CompleteRegistration()
    {
        // Validation âge client-side
        var age = (DateTime.Now - BirthDate).Days / 365.25;
        if (age <= 14)
        {
            ErrorMessage = "Vous devez avoir plus de 14 ans pour utiliser SyncTrip.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Le pseudo est obligatoire.";
            return;
        }

        if (Username.Length > 50)
        {
            ErrorMessage = "Le pseudo ne peut pas dépasser 50 caractères.";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var request = new CompleteRegistrationRequest
            {
                Email = Email,
                Username = Username.Trim(),
                FirstName = string.IsNullOrWhiteSpace(FirstName) ? null : FirstName.Trim(),
                LastName = string.IsNullOrWhiteSpace(LastName) ? null : LastName.Trim(),
                BirthDate = DateOnly.FromDateTime(BirthDate)
            };

            var jwt = await _authService.CompleteRegistrationAsync(request);

            if (!string.IsNullOrEmpty(jwt))
            {
                // Navigation vers page principale
                await Shell.Current.GoToAsync("//main");
            }
            else
            {
                ErrorMessage = "Une erreur est survenue lors de l'inscription.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
