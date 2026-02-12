using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.Mobile.Core.Services;
using SyncTrip.Shared.DTOs.Users;

namespace SyncTrip.Mobile.Features.Profile.ViewModels;

/// <summary>
/// ViewModel pour la page de profil utilisateur.
/// Permet d'afficher et de modifier les informations de profil (nom, prénom, date de naissance, permis).
/// </summary>
public partial class ProfileViewModel : ObservableObject
{
    private readonly IUserService _userService;
    private readonly IAuthenticationService _authService;

    /// <summary>
    /// Identifiant de l'utilisateur.
    /// </summary>
    [ObservableProperty]
    private Guid userId;

    /// <summary>
    /// Adresse email (non modifiable).
    /// </summary>
    [ObservableProperty]
    private string email = string.Empty;

    /// <summary>
    /// Nom d'utilisateur (pseudo).
    /// </summary>
    [ObservableProperty]
    private string username = string.Empty;

    /// <summary>
    /// Prénom de l'utilisateur (facultatif).
    /// </summary>
    [ObservableProperty]
    private string? firstName;

    /// <summary>
    /// Nom de famille de l'utilisateur (facultatif).
    /// </summary>
    [ObservableProperty]
    private string? lastName;

    /// <summary>
    /// Date de naissance de l'utilisateur.
    /// </summary>
    [ObservableProperty]
    private DateTime birthDate = DateTime.Now.AddYears(-18);

    /// <summary>
    /// URL de l'avatar (facultatif).
    /// </summary>
    [ObservableProperty]
    private string? avatarUrl;

    /// <summary>
    /// Âge calculé de l'utilisateur.
    /// </summary>
    [ObservableProperty]
    private int age;

    /// <summary>
    /// Liste des permis détenus par l'utilisateur (LicenseType).
    /// </summary>
    [ObservableProperty]
    private List<int> licenseTypes = new();

    /// <summary>
    /// Indique si une opération de chargement est en cours.
    /// </summary>
    [ObservableProperty]
    private bool isLoading;

    /// <summary>
    /// Indique si une opération de sauvegarde est en cours.
    /// </summary>
    [ObservableProperty]
    private bool isSaving;

    /// <summary>
    /// Message d'erreur à afficher.
    /// </summary>
    [ObservableProperty]
    private string? errorMessage;

    /// <summary>
    /// Message de succès à afficher.
    /// </summary>
    [ObservableProperty]
    private string? successMessage;

    /// <summary>
    /// Indique si le profil est en mode édition.
    /// </summary>
    [ObservableProperty]
    private bool isEditing;

    /// <summary>
    /// Date maximale pour le DatePicker (14 ans en arrière).
    /// </summary>
    public DateTime MaximumDate => DateTime.Now.AddYears(-14);

    /// <summary>
    /// Initialise une nouvelle instance du ViewModel.
    /// </summary>
    /// <param name="userService">Service utilisateur.</param>
    /// <param name="authService">Service d'authentification.</param>
    public ProfileViewModel(IUserService userService, IAuthenticationService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    /// <summary>
    /// Charge le profil de l'utilisateur connecté.
    /// </summary>
    [RelayCommand]
    private async Task LoadProfile()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var profile = await _userService.GetProfileAsync();

            if (profile != null)
            {
                UserId = profile.Id;
                Email = profile.Email;
                Username = profile.Username;
                FirstName = profile.FirstName;
                LastName = profile.LastName;
                BirthDate = profile.BirthDate.ToDateTime(TimeOnly.MinValue);
                AvatarUrl = profile.AvatarUrl;
                Age = profile.Age;
                LicenseTypes = new List<int>(profile.LicenseTypes);
            }
            else
            {
                ErrorMessage = "Impossible de charger le profil.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Active le mode édition du profil.
    /// </summary>
    [RelayCommand]
    private void StartEdit()
    {
        IsEditing = true;
        ErrorMessage = null;
        SuccessMessage = null;
    }

    /// <summary>
    /// Annule l'édition et recharge le profil.
    /// </summary>
    [RelayCommand]
    private async Task CancelEdit()
    {
        IsEditing = false;
        await LoadProfile();
    }

    /// <summary>
    /// Sauvegarde les modifications du profil.
    /// </summary>
    [RelayCommand]
    private async Task SaveProfile()
    {
        // Validation client-side
        var ageCheck = (DateTime.Now - BirthDate).Days / 365.25;
        if (ageCheck <= 14)
        {
            ErrorMessage = "Vous devez avoir plus de 14 ans.";
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
            IsSaving = true;
            ErrorMessage = null;
            SuccessMessage = null;

            var request = new UpdateUserProfileRequest
            {
                Username = Username.Trim(),
                FirstName = string.IsNullOrWhiteSpace(FirstName) ? null : FirstName.Trim(),
                LastName = string.IsNullOrWhiteSpace(LastName) ? null : LastName.Trim(),
                BirthDate = DateOnly.FromDateTime(BirthDate),
                AvatarUrl = string.IsNullOrWhiteSpace(AvatarUrl) ? null : AvatarUrl.Trim(),
                LicenseTypes = LicenseTypes
            };

            var success = await _userService.UpdateProfileAsync(request);

            if (success)
            {
                SuccessMessage = "Profil mis à jour avec succès.";
                IsEditing = false;
                await LoadProfile(); // Recharger pour avoir les données à jour
            }
            else
            {
                ErrorMessage = "Impossible de mettre à jour le profil.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
        finally
        {
            IsSaving = false;
        }
    }

    /// <summary>
    /// Ajoute un permis à la liste.
    /// </summary>
    /// <param name="licenseType">Type de permis à ajouter.</param>
    [RelayCommand]
    private void AddLicense(int licenseType)
    {
        if (!LicenseTypes.Contains(licenseType))
        {
            LicenseTypes.Add(licenseType);
            OnPropertyChanged(nameof(LicenseTypes));
        }
    }

    /// <summary>
    /// Retire un permis de la liste.
    /// </summary>
    /// <param name="licenseType">Type de permis à retirer.</param>
    [RelayCommand]
    private void RemoveLicense(int licenseType)
    {
        if (LicenseTypes.Contains(licenseType))
        {
            LicenseTypes.Remove(licenseType);
            OnPropertyChanged(nameof(LicenseTypes));
        }
    }

    /// <summary>
    /// Déconnecte l'utilisateur.
    /// </summary>
    [RelayCommand]
    private async Task Logout()
    {
        try
        {
            await _authService.ClearTokenAsync();
            await Shell.Current.GoToAsync("//login", true);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors de la déconnexion: {ex.Message}";
        }
    }
}
