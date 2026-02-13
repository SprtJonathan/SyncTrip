using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.App.Core.Platform;
using SyncTrip.App.Core.Services;
using SyncTrip.Shared.DTOs.Auth;

namespace SyncTrip.App.Features.Authentication.ViewModels;

public partial class RegistrationViewModel : ObservableObject
{
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string? firstName;

    [ObservableProperty]
    private string? lastName;

    [ObservableProperty]
    private DateTime birthDate = DateTime.Now.AddYears(-15);

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;

    public DateTime MaximumDate => DateTime.Now.AddYears(-14);

    public RegistrationViewModel(IAuthenticationService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
    }

    public void Initialize(string email)
    {
        Email = email;
    }

    [RelayCommand]
    private async Task CompleteRegistration()
    {
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
            ErrorMessage = "Le pseudo ne peut pas depasser 50 caracteres.";
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
                await _navigationService.NavigateToAsync("main");
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
