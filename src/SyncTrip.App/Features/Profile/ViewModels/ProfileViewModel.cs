using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.App.Core.Platform;
using SyncTrip.App.Core.Services;
using SyncTrip.Shared.DTOs.Users;

namespace SyncTrip.App.Features.Profile.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IUserService _userService;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Guid userId;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string? firstName;

    [ObservableProperty]
    private string? lastName;

    [ObservableProperty]
    private DateTime birthDate = DateTime.Now.AddYears(-18);

    [ObservableProperty]
    private string? avatarUrl;

    [ObservableProperty]
    private int age;

    [ObservableProperty]
    private List<int> licenseTypes = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isSaving;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? successMessage;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private bool hasLicenseB;

    [ObservableProperty]
    private bool hasLicenseA;

    [ObservableProperty]
    private bool hasLicenseC;

    [ObservableProperty]
    private bool hasLicenseD;

    public DateTime MaximumDate => DateTime.Now.AddYears(-14);

    public ProfileViewModel(IUserService userService, IAuthenticationService authService, INavigationService navigationService)
    {
        _userService = userService;
        _authService = authService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    public async Task LoadProfile()
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
                HasLicenseB = LicenseTypes.Contains(1);
                HasLicenseA = LicenseTypes.Contains(2);
                HasLicenseC = LicenseTypes.Contains(3);
                HasLicenseD = LicenseTypes.Contains(4);
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

    [RelayCommand]
    private void StartEdit()
    {
        IsEditing = true;
        ErrorMessage = null;
        SuccessMessage = null;
    }

    [RelayCommand]
    private async Task CancelEdit()
    {
        IsEditing = false;
        await LoadProfile();
    }

    [RelayCommand]
    private async Task SaveProfile()
    {
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
            ErrorMessage = "Le pseudo ne peut pas depasser 50 caracteres.";
            return;
        }

        try
        {
            IsSaving = true;
            ErrorMessage = null;
            SuccessMessage = null;

            SyncLicensesFromCheckboxes();

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
                SuccessMessage = "Profil mis a jour avec succes.";
                IsEditing = false;
                await LoadProfile();
            }
            else
            {
                ErrorMessage = "Impossible de mettre a jour le profil.";
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

    private void SyncLicensesFromCheckboxes()
    {
        LicenseTypes = new List<int>();
        if (HasLicenseB) LicenseTypes.Add(1);
        if (HasLicenseA) LicenseTypes.Add(2);
        if (HasLicenseC) LicenseTypes.Add(3);
        if (HasLicenseD) LicenseTypes.Add(4);
    }

    [RelayCommand]
    private async Task Logout()
    {
        try
        {
            await _authService.ClearTokenAsync();
            await _navigationService.NavigateToAsync("login");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors de la deconnexion: {ex.Message}";
        }
    }
}
